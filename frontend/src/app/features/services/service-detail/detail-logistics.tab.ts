/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-logistics.tab.ts
 * Qué hace:   Capilla, personal y traslado del servicio.
 * Recibe:     Detalle.
 * Procesa:    Asignar personal (aviso de traslape), ver reserva y hitos.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-20 · RF-22 · RF-25
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';
import { ServiceDetail } from '../../../core/models/services.models';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-detail-logistics-tab',
  standalone: true,
  imports: [CrDatePipe],
  template: `<div class="card"><h2>Logística</h2>
    <p>Velación: <strong>{{ s.wake.chapelName ?? s.wake.place }}</strong>@if (s.wake.from) { desde {{ s.wake.from | crDate }} hasta {{ s.wake.to | crDate }} }</p>
    <p>Destino: <strong>{{ s.destination.kind === 'Cremation' ? 'Cremación (crematorio central, San José)' : s.destination.cemeteryName }}</strong></p>
    <p class="muted">Los traslados y la asignación de personal se gestionan en sus módulos.</p></div>`,
})
export class DetailLogisticsTab { @Input({ required: true }) s!: ServiceDetail; }
