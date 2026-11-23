/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-providers.tab.ts
 * Qué hace:   Ofertas y adjudicaciones por componente.
 * Recibe:     Board + ofertas.
 * Procesa:    Muestra 'Aceptado por Floristería X, ₡85 000, 14:32' en vivo con OfferAwarded.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-15 a RF-17
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';
import { ServiceBoardComponent } from './service-board.component';

@Component({ selector: 'app-detail-providers-tab', standalone: true, imports: [ServiceBoardComponent], template: `<app-service-board [serviceId]="serviceId" />` })
export class DetailProvidersTab { @Input({ required: true }) serviceId!: string; }
