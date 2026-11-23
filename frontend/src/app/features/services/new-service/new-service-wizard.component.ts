/**
 * Archivo:    frontend/src/app/features/services/new-service/new-service-wizard.component.ts
 * Qué hace:   Asistente 'Nuevo servicio' de 6 pasos.
 * Recibe:     Catálogo, stock y capillas de la sede actual.
 * Procesa:    FormGroup tipado con un subgrupo por paso; FormDraftService (clave 'nuevo-servicio'); se
 *               puede volver atrás sin perder datos; al terminar arma CreateServiceRequest, sube el acta
 *               y limpia el borrador.
 * Devuelve:   Servicio creado → navega al detalle en una pestaña ERP.
 * Se conecta: → step-*.component, total-panel, ServicesApi, FormDraftService, unsaved-changes.guard
 * Reglas:     RF-01 a RF-07 · RF-36 · RNF-11 (< 10 min)
 * Estado:     Completo
 *
 * Contrato:
 *   submit(): void
 *     Recibe:     formulario válido
 *     Procesa:    POST /api/services → POST death-certificate (si hay archivo) → clear borrador
 *     Devuelve:   navegación a /servicios/:id
 */

import { Component, OnDestroy, OnInit, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription, forkJoin, of, switchMap } from 'rxjs';
import { CatalogApi } from '../../../core/api/catalog.api';
import { LogisticsApi } from '../../../core/api/logistics.api';
import { MetaApi } from '../../../core/api/meta.api';
import { ServicesApi } from '../../../core/api/services.api';
import { FormDraftService } from '../../../core/forms/form-draft.service';
import { applyServerErrors } from '../../../core/forms/server-errors';
import { HasUnsavedChanges } from '../../../core/forms/unsaved-changes.guard';
import { PageHeaderComponent } from '../../../core/layout/page-header.component';
import { CasketModel, Component as Comp, Package } from '../../../core/models/catalog.models';
import { ChapelCalendar } from '../../../core/models/logistics.models';
import { Option } from '../../../core/models/meta.models';
import { CreateServiceRequest, LineInput } from '../../../core/models/services.models';
import { BranchContextService } from '../../../core/state/branch-context.service';
import { OpenTabsService } from '../../../core/state/open-tabs.service';
import { StepCasketComponent } from './step-casket.component';
import { StepFamilyComponent } from './step-family.component';
import { StepPackageComponent } from './step-package.component';
import { StepPaymentDocsComponent } from './step-payment-docs.component';
import { StepTypeDeceasedComponent } from './step-type-deceased.component';
import { StepWakeDestinationComponent } from './step-wake-destination.component';
import { PanelLine, TotalPanelComponent } from './total-panel.component';
import { calcTotals } from './invoice-math';

const DRAFT_KEY = 'nuevo-servicio';
const STEPS = ['Fallecido', 'Familia', 'Paquete', 'Personalizar', 'Velación y destino', 'Pago y acta'];
const ID = /^\d-\d{4}-\d{4}$/;

@Component({
  selector: 'app-new-service-wizard',
  standalone: true,
  imports: [PageHeaderComponent, TotalPanelComponent, StepTypeDeceasedComponent, StepFamilyComponent, StepPackageComponent, StepCasketComponent, StepWakeDestinationComponent, StepPaymentDocsComponent],
  template: `
    <app-page-header title="Nuevo servicio" description="Registro guiado en 6 pasos. El borrador se guarda solo." [rf]="['RF-01','RF-02','RF-04','RF-05','RF-07']" />
    <ol class="steps">@for (s of steps; track s; let i = $index) {
      <li [class.on]="i === step()" [class.done]="i < step()"><button type="button" (click)="goTo(i)" [disabled]="i > step()">{{ i + 1 }} · {{ s }}</button></li> }</ol>
    <div class="layout">
      <section class="card">
        @switch (step()) {
          @case (0) { <app-step-type-deceased [form]="form" /> }
          @case (1) { <app-step-family [form]="form" /> }
          @case (2) { <app-step-package [packages]="packages()" [selectedId]="form.value.basePackageId ?? null" (pick)="pickPackage($event)" /> }
          @case (3) { <app-step-casket [lines]="lines()" [components]="components()" [caskets]="caskets()" [custom]="custom()" (add)="addLine($event)" (remove)="removeLine($event)" (chooseCasket)="chooseCasket($event)" /> }
          @case (4) { <app-step-wake-destination [form]="form" [calendar]="calendar()" (changed)="loadCalendar()" /> }
          @case (5) { <app-step-payment-docs [form]="form" [insurers]="insurers()" [totals]="totals()" (file)="file = $event" /> }
        }
        <div class="nav">
          <button class="btn" type="button" [disabled]="step() === 0" (click)="back()">Anterior</button>
          <span class="spacer"></span>
          @if (step() < 5) { <button class="btn primary" type="button" (click)="next()">Siguiente</button> }
          @else { <button class="btn primary" type="button" [disabled]="saving()" (click)="submit()">{{ saving() ? 'Guardando…' : 'Registrar servicio' }}</button> }
        </div>
      </section>
      <app-total-panel [value]="panelLines()" />
    </div>`,
  styles: [`.steps { display: flex; gap: 4px; list-style: none; padding: 0; margin: 0 0 16px; flex-wrap: wrap; } .steps button { background: var(--surface); border: 1px solid var(--border); color: var(--text-muted); border-radius: var(--radius); height: 32px; padding: 0 12px; font: inherit; cursor: pointer; }
    .steps .on button { border-color: var(--accent); color: var(--text); background: var(--accent-soft); } .steps .done button { color: var(--accent-text); } .steps button:disabled { cursor: default; opacity: .6; }
    .layout { display: grid; grid-template-columns: 1fr 300px; gap: 16px; align-items: start; } .nav { display: flex; margin-top: 20px; } .spacer { flex: 1; }`],
})
export class NewServiceWizardComponent implements OnInit, OnDestroy, HasUnsavedChanges {
  private fb = inject(FormBuilder);
  private catalog = inject(CatalogApi);
  private logistics = inject(LogisticsApi);
  private meta = inject(MetaApi);
  private api = inject(ServicesApi);
  private drafts = inject(FormDraftService);
  private branch = inject(BranchContextService);
  private tabs = inject(OpenTabsService);
  private router = inject(Router);
  private snack = inject(MatSnackBar);
  private sub?: Subscription;
  private submitted = false;

  readonly steps = STEPS;
  readonly step = signal(0);
  readonly saving = signal(false);
  readonly packages = signal<Package[]>([]);
  readonly components = signal<Comp[]>([]);
  readonly caskets = signal<CasketModel[]>([]);
  readonly insurers = signal<Option[]>([]);
  readonly calendar = signal<ChapelCalendar | null>(null);
  readonly lines = signal<LineInput[]>([]);
  file: File | null = null;

  readonly form = this.fb.group({
    kind: ['Immediate', Validators.required],
    deceased: this.fb.group({ fullName: ['', Validators.required], nationalId: ['', [Validators.required, Validators.pattern(ID)]], birthDate: ['', Validators.required], deathDate: [''] }),
    family: this.fb.group({ fullName: ['', Validators.required], nationalId: ['', [Validators.required, Validators.pattern(ID)]], relationship: ['Hijo', Validators.required],
      phone: ['', [Validators.required, Validators.pattern(/^\d{4}-?\d{4}$/)]], email: ['', [Validators.required, Validators.email]], insuranceProspectConsent: [false] }),
    basePackageId: [null as number | null],
    wake: this.fb.group({ date: ['', Validators.required], time: ['10:00', Validators.required], place: ['Chapel'], address: [''], chapelId: [null as number | null], destination: ['Cemetery'], cemeteryName: [''] }),
    payment: this.fb.group({ payer: ['Family'], insurerId: [''], policyNumber: [''], certNumber: [''] }),
  });

  private priced = computed<(PanelLine & { componentId: number })[]>(() => {
    const comps = this.components(); const models = this.caskets();
    return this.lines().map(l => {
      const c = comps.find(x => x.id === l.componentId);
      const m = l.casketModelId ? models.find(x => x.id === l.casketModelId) : undefined;
      return { componentId: l.componentId, name: m ? `${c?.name} · ${m.name}` : c?.name ?? '', qty: l.qty, unitPriceCrc: m?.priceCrc ?? c?.priceCrc ?? 0, taxExemptPct: c?.taxExemptPct ?? 0 };
    });
  });
  readonly panelLines = computed<PanelLine[]>(() => this.priced());
  readonly totals = computed(() => calcTotals(this.priced()));
  readonly custom = computed(() => {
    const pkg = this.packages().find(p => p.id === this.form.value.basePackageId);
    if (!pkg) return false;
    const a = new Map(pkg.items.map(i => [i.componentId, i.qty])); const b = new Map(this.lines().map(l => [l.componentId, l.qty]));
    return a.size !== b.size || [...a].some(([k, q]) => b.get(k) !== q);
  });

  constructor() {
    effect(() => { this.branch.branchId(); this.reloadCatalog(); });
  }

  ngOnInit(): void {
    this.sub = this.drafts.attach(this.form, DRAFT_KEY);
    try { const raw = sessionStorage.getItem('acompana.draft.nuevo-servicio.lines'); if (raw) this.lines.set(JSON.parse(raw)); } catch { /* sin borrador */ }
    this.meta.insurers().subscribe(i => { this.insurers.set(i); if (!this.form.value.payment?.insurerId && i.length) this.form.get('payment.insurerId')?.setValue(i[0].value); });
    this.form.get('kind')?.valueChanges.subscribe(k => {
      const d = this.form.get('deceased.deathDate')!;
      d.setValidators(k === 'Immediate' ? [Validators.required] : []); d.updateValueAndValidity();
    });
    this.form.get('kind')?.updateValueAndValidity();
    this.form.get('wake.place')?.valueChanges.subscribe(p => {
      const a = this.form.get('wake.address')!; a.setValidators(p === 'Home' ? [Validators.required] : []); a.updateValueAndValidity(); this.loadCalendar();
    });
    this.form.get('wake.destination')?.valueChanges.subscribe(d => {
      const c = this.form.get('wake.cemeteryName')!; c.setValidators(d === 'Cemetery' ? [Validators.required] : []); c.updateValueAndValidity();
    });
    this.form.get('wake.destination')?.updateValueAndValidity();
    this.form.get('payment.payer')?.valueChanges.subscribe(p => {
      for (const k of ['insurerId', 'policyNumber']) { const c = this.form.get('payment.' + k)!; c.setValidators(p === 'Insurer' ? [Validators.required] : []); c.updateValueAndValidity(); }
    });
    const d = new Date(Date.now() + 86400000); this.form.get('wake.date')?.setValue(this.form.value.wake?.date || d.toISOString().slice(0, 10), { emitEvent: false });
  }

  ngOnDestroy(): void { this.sub?.unsubscribe(); }

  hasUnsavedChanges(): boolean { return !this.submitted && this.form.dirty; }

  private reloadCatalog(): void {
    const b = this.branch.branchId();
    forkJoin([this.catalog.packages(), this.catalog.components({ active: true }), this.catalog.casketModels(b)]).subscribe(([p, c, m]) => {
      this.packages.set(p); this.components.set(c); this.caskets.set(m);
    });
    this.loadCalendar();
  }

  private persistLines(): void { try { sessionStorage.setItem('acompana.draft.nuevo-servicio.lines', JSON.stringify(this.lines())); } catch { /* sin almacenamiento */ } }

  pickPackage(p: Package): void {
    this.form.get('basePackageId')?.setValue(p.id); this.form.markAsDirty();
    this.lines.set(p.items.map(i => ({ componentId: i.componentId, casketModelId: null, qty: i.qty }))); this.persistLines();
  }
  addLine(id: number): void { this.lines.update(l => [...l, { componentId: id, casketModelId: null, qty: 1 }]); this.form.markAsDirty(); this.persistLines(); }
  removeLine(id: number): void { this.lines.update(l => l.filter(x => x.componentId !== id)); this.form.markAsDirty(); this.persistLines(); }
  chooseCasket(modelId: number): void {
    const casketComp = new Set(this.components().filter(c => c.category === 'Casket').map(c => c.id));
    this.lines.update(l => l.map(x => casketComp.has(x.componentId) ? { ...x, casketModelId: modelId } : x)); this.form.markAsDirty(); this.persistLines();
  }

  private iso(): string { const w = this.form.value.wake!; return `${w.date}T${w.time}:00-06:00`; }

  loadCalendar(): void {
    const w = this.form.value.wake; const b = this.branch.branchId();
    if (!w?.date || !w.time || w.place !== 'Chapel' || b === null) { this.calendar.set(null); return; }
    const from = new Date(this.iso());
    this.logistics.chapelCalendar(b, from.toISOString(), new Date(from.getTime() + 24 * 3600000).toISOString()).subscribe(c => this.calendar.set(c));
  }

  private groupFor(step: number): string[] {
    return [['kind', 'deceased'], ['family'], [], [], ['wake'], ['payment']][step];
  }

  private validStep(step: number): boolean {
    if (step === 2 && !this.form.value.basePackageId) { this.snack.open('Seleccione un paquete.', 'Cerrar', { duration: 3000, panelClass: 'error' }); return false; }
    if (step === 3 && this.lines().length === 0) { this.snack.open('El servicio necesita al menos un componente.', 'Cerrar', { duration: 3000, panelClass: 'error' }); return false; }
    let ok = true;
    for (const g of this.groupFor(step)) { const c = this.form.get(g)!; c.markAllAsTouched(); if (c.invalid) ok = false; }
    if (step === 4 && this.form.value.wake?.place === 'Chapel' && !this.form.value.wake.chapelId) { this.form.get('wake.chapelId')?.setErrors({ required: true }); this.form.get('wake.chapelId')?.markAsTouched(); ok = false; }
    return ok;
  }

  next(): void { if (this.validStep(this.step())) this.step.update(s => s + 1); }
  back(): void { this.step.update(s => Math.max(0, s - 1)); }
  goTo(i: number): void { if (i <= this.step()) this.step.set(i); }

  submit(): void {
    for (let i = 0; i < 5; i++) if (!this.validStep(i)) { this.step.set(i); return; }
    if (!this.validStep(5)) return;
    const v = this.form.getRawValue();
    const req: CreateServiceRequest = {
      kind: v.kind as never, branchId: this.branch.branchId() ?? 0,
      deceased: { fullName: v.deceased.fullName!, nationalId: v.deceased.nationalId!, birthDate: v.deceased.birthDate!, deathDate: v.kind === 'Immediate' ? v.deceased.deathDate || null : null },
      family: v.family as never, basePackageId: v.basePackageId!, lines: this.lines(),
      wakePlace: v.wake.place as never, wakeAddress: v.wake.place === 'Home' ? v.wake.address : null, chapelId: v.wake.place === 'Chapel' ? v.wake.chapelId : null,
      finalDestination: v.wake.destination as never, cemeteryName: v.wake.destination === 'Cemetery' ? v.wake.cemeteryName : null, scheduledAt: this.iso(),
      payer: v.payment.payer as never, insurerId: v.payment.payer === 'Insurer' ? Number(v.payment.insurerId) : null, policyNumber: v.payment.payer === 'Insurer' ? v.payment.policyNumber : null,
    };
    this.saving.set(true);
    this.api.create(req).pipe(
      switchMap(s => (this.file && v.payment.certNumber ? this.api.uploadCertificate(s.id, v.payment.certNumber, this.file).pipe(switchMap(() => of(s))) : of(s))),
    ).subscribe({
      next: s => {
        this.submitted = true; this.drafts.clear(DRAFT_KEY);
        try { sessionStorage.removeItem('acompana.draft.nuevo-servicio.lines'); } catch { /* sin almacenamiento */ }
        this.snack.open(`Servicio ${s.code} registrado.`, 'Cerrar', { duration: 4000, panelClass: 'success' });
        this.tabs.open(s.id, s.code);
      },
      error: e => {
        this.saving.set(false);
        const unmatched = applyServerErrors(this.form, e.error);
        if (unmatched.length) this.snack.open(unmatched[0], 'Cerrar', { duration: 6000, panelClass: 'error' });
        if (e.status === 409) { this.step.set(4); this.loadCalendar(); }
      },
    });
  }
}
