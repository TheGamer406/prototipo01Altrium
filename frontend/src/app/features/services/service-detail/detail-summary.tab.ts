/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-summary.tab.ts
 * Qué hace:   Resumen y tablero del servicio.
 * Recibe:     Detalle + board.
 * Procesa:    Datos clave + service-board + línea de tiempo.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-19
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';
import { ServiceDetail } from '../../../core/models/services.models';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

const PLACE: Record<string, string> = { Chapel: 'Capilla', Church: 'Iglesia', Home: 'Casa de habitación' };

@Component({
  selector: 'app-detail-summary-tab',
  standalone: true,
  imports: [CrDatePipe],
  template: `
    <div class="grid">
      <section class="card"><h2>Fallecido</h2><dl><dt>Nombre</dt><dd>{{ s.deceased.fullName }}</dd><dt>Cédula</dt><dd>{{ s.deceased.nationalId }}</dd>
        <dt>Nacimiento</dt><dd>{{ s.deceased.birthDate }}</dd><dt>Defunción</dt><dd>{{ s.deceased.deathDate ?? 'Pendiente (servicio anticipado)' }}</dd></dl></section>
      <section class="card"><h2>Familia responsable</h2><dl><dt>Nombre</dt><dd>{{ s.family.fullName }}</dd><dt>Parentesco</dt><dd>{{ s.family.relationship }}</dd>
        <dt>Teléfono</dt><dd>{{ s.family.phone }}</dd><dt>Correo</dt><dd>{{ s.family.email }}</dd><dt>Consentimiento seguros</dt><dd>{{ s.family.insuranceProspectConsent ? 'Sí' : 'No' }}</dd></dl></section>
      <section class="card"><h2>Velación y destino</h2><dl><dt>Fecha</dt><dd>{{ s.scheduledAt | crDate }}</dd><dt>Lugar</dt><dd>{{ place(s.wake.place) }}{{ s.wake.chapelName ? ' · ' + s.wake.chapelName : '' }}{{ s.wake.address ? ' · ' + s.wake.address : '' }}</dd>
        <dt>Destino</dt><dd>{{ s.destination.kind === 'Cremation' ? 'Cremación' : 'Cementerio · ' + s.destination.cemeteryName }}</dd></dl></section>
      <section class="card"><h2>Pagador</h2><dl><dt>Paga</dt><dd>{{ s.payer.payer === 'Insurer' ? 'Aseguradora' : 'Familia' }}</dd>
        @if (s.payer.insurerName) { <dt>Aseguradora</dt><dd>{{ s.payer.insurerName }} · póliza {{ s.payer.policyNumber }}</dd> }</dl></section>
    </div>`,
  styles: [`.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; } dl { display: grid; grid-template-columns: 150px 1fr; gap: 6px 12px; margin: 10px 0 0; } dt { color: var(--text-muted); } dd { margin: 0; }
    @media (max-width: 1024px) { .grid { grid-template-columns: 1fr; } }`],
})
export class DetailSummaryTab {
  @Input({ required: true }) s!: ServiceDetail;
  place(p: string): string { return PLACE[p] ?? p; }
}
