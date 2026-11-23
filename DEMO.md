# Guion del demo — Acompaña (5 a 7 minutos)

Prototipo de **ATRIUM Solutions** para Funeraria Los Cipreses. Todos los datos son ficticios.

## Antes de empezar

1. Abrir `http://<IP_DEL_SERVIDOR>:5471` con el usuario y la clave de `.env`.
2. Activar **Admin** en la barra superior y usar **Reiniciar demo** para partir de los datos iniciales (tarda unos 8 segundos).
3. Tener una segunda pestaña del navegador lista para el rol Proveedor.

Los selectores **Sede** y **Rol** de la barra superior simulan el login (no hay usuarios reales).

## Recorrido

| # | Qué mostrar | Cómo |
|---|---|---|
| 1 | **Vista central** | Rol Gerencia, sede Todas → *Vista central*: mapa de las 28 sedes, servicios en curso, capillas ocupadas, cofres bajo mínimo y alertas abiertas. |
| 2 | **Nuevo servicio** (menos de 10 min, RNF-11) | Rol Asesor funerario, sede Heredia → *Servicios* → *Nuevo servicio*. Paso 3: paquete **Tradicional**. Paso 4: quitar el catering (aparece «Paquete personalizado»), elegir un cofre con imagen y existencias. Paso 5: capilla disponible. Paso 6: número de acta y archivo. El total se actualiza en cada paso. |
| 3 | **Factura** | Detalle del servicio → pestaña *Factura* → *Enviar al core contable* → queda aceptada. Modo admin: **Forzar rechazo de la próxima factura**, enviar otra y ver el motivo; corregir la cédula de la familia y reenviar. |
| 4 | **Pago y proveedores** | *Registrar pago*: las ofertas salen solas a los proveedores de la zona. Segunda pestaña: rol Proveedor, elegir una floristería → *Bandeja del proveedor* → **Aceptar**. En el detalle del servicio, pestaña *Proveedores*, el tablero se actualiza en vivo. Modo admin: **Simular aceptación simultánea** → gana uno solo («Ya fue asignada» para el otro). |
| 5 | **Caso de las flores** | Gerencia → *Bitácora*: se ve quién quitó la corona de flores del servicio de Heredia y cuándo. |
| 6 | **Inventario** | Rol Encargado de sede → *Inventario de cofres*: el cofre vendido bajó el stock; si llegó al mínimo hay alerta y una solicitud a la bodega central (pestañas *Alertas* y *Solicitudes*). |
| 7 | **Cremación** | Rol Operador de crematorio → *Cremación*: mover una tarjeta de «En horno» a «Empacado»; al entregar se pide quién recibe las cenizas. |
| 8 | **Analítica** | Gerencia → *Analítica*: pico de diciembre y enero, ventas por paquete, etapa más lenta por sede. |
| 9 | **Traslados y chofer** (opcional) | Encargado → *Traslados*: mapa con un traslado demorado. Rol Chofer → *App del chofer*: activar «Sin señal», marcar un hito y desactivarlo: se sincroniza con su hora real y sin duplicarse. |
| 10 | **Defensa técnica** | `http://<IP_DEL_SERVIDOR>:5472` grafo del proyecto (frontend, backend y base de datos) · `http://<IP_DEL_SERVIDOR>:5473` pgAdmin con la conexión preconfigurada (ERD en vivo). |
| 11 | **Cierre** | Admin → **Reiniciar demo** deja todo como al inicio. |

## Datos de partida que sirven para el guion

- **Sedes:** 28 (12 en el GAM y 16 fuera). Heredia tiene 3 capillas.
- **Hoy:** 2 servicios anticipados, 3 sin acta, 2 en facturación (uno rechazado), 4 pagados con ofertas abiertas (uno con un componente urgente y otro con la oferta vencida y su alerta), 6 en curso y 2 finalizados.
- **Inventario:** Heredia queda a una venta del mínimo en los cofres COF-002 y COF-003; San José Centro, Cartago y Liberia ya están en el mínimo o debajo.
- **Cremación:** 6 cuerpos en la cola del crematorio central.
- **Traslados:** uno con demora y uno marcado sin señal y sincronizado.

## Simulaciones del modo admin

- Reiniciar demo · Refrescar analítica.
- Simular aceptación simultánea de dos proveedores.
- Forzar el rechazo de la próxima factura.
- Core contable caído 5 minutos (la factura queda enviada y se reintenta).
