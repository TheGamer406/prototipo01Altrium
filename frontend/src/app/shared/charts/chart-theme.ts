/**
 * Archivo:    frontend/src/app/shared/charts/chart-theme.ts
 * Qué hace:   Tema de ECharts con los tokens del sistema.
 * Recibe:     —
 * Procesa:    Paleta: #176B87, #4F8A8B, #7A8FA6, #9C8E74, #6F9A80, #8C7A99; ejes y grilla en --border;
 *               texto --text-muted; fuente Inter.
 * Devuelve:   Objeto de tema registrado como 'acompana'.
 * Se conecta: ← analytics, central-view, home
 * Estado:     Completo
 */

export const CHART_THEME_NAME = 'acompana';

export const chartTheme = {
  color: ['#176b87', '#4f8a8b', '#7a8fa6', '#9c8e74', '#6f9a80', '#8c7a99'],
  backgroundColor: 'transparent',
  textStyle: { fontFamily: 'Inter, system-ui, sans-serif', color: '#8a99a5' },
  title: { textStyle: { color: '#d7dee3' } },
  legend: { textStyle: { color: '#8a99a5' } },
  categoryAxis: { axisLine: { lineStyle: { color: '#30404a' } }, axisTick: { show: false }, axisLabel: { color: '#8a99a5' }, splitLine: { show: false } },
  valueAxis: { axisLine: { show: false }, axisTick: { show: false }, axisLabel: { color: '#8a99a5' }, splitLine: { lineStyle: { color: '#30404a' } } },
  tooltip: { backgroundColor: '#243039', borderColor: '#30404a', textStyle: { color: '#d7dee3' } },
};
