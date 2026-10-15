using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiComanderaApp.Core.Domain.Models;

namespace MiComanderaApp.Core.Application.Services
{
    /// <summary>
    /// Genera los textos de facturas, prefacturas y comprobantes de pago
    /// con formato para impresora térmica (42 caracteres de ancho).
    /// </summary>
    public static class DocumentosVenta
    {
        private const int Ancho = 42;
        private static readonly string Separador = new string('-', Ancho);

        public static string BuildPrefactura(VentaDetalleModel venta) =>
            Encabezado(venta, "PREFACTURA", "Documento no fiscal - NO VÁLIDO COMO FACTURA")
            + Cuerpo(venta)
            + Totales(venta)
            + Pie(venta, $"PREFACTURA #{venta.VentaId}");

        public static string BuildFactura(VentaDetalleModel venta) =>
            Encabezado(venta, "FACTURA", venta.TienePagos ? "Documento fiscal" : "Documento no cobrado")
            + DatosCliente(venta)
            + Cuerpo(venta)
            + Totales(venta)
            + (venta.TienePagos ? Pagos(venta) : "")
            + Pie(venta, $"FACTURA #{venta.VentaId}");

        public static string BuildComprobantePago(VentaDetalleModel venta) =>
            Encabezado(venta, "COMPROBANTE DE PAGO", "Gracias por su compra")
            + DatosCliente(venta)
            + Cuerpo(venta)
            + Totales(venta)
            + Pagos(venta)
            + Pie(venta, $"PAGO #{venta.VentaId}");

        private static string Encabezado(VentaDetalleModel venta, string titulo, string subtitulo)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Centrar("RESTAURANTE"));
            sb.AppendLine(Centrar(titulo));
            sb.AppendLine(Centrar(subtitulo));
            sb.AppendLine(Separador);
            sb.AppendLine($"Mesa: {venta.NumeroMesa,-10}Fecha: {venta.FechaApertura.ToLocalTime():dd/MM/yyyy}");
            sb.AppendLine($"Hora:  {venta.FechaApertura.ToLocalTime(),-10:HH:mm}Caja:  Cajero");
            sb.AppendLine($"Venta: #{venta.VentaId,-10}Mesero: {venta.NombreMesero}");
            if (!string.IsNullOrWhiteSpace(venta.AliasVenta))
                sb.AppendLine($"Alias: {venta.AliasVenta}");
            sb.AppendLine(Separador);
            return sb.ToString();
        }

        private static string DatosCliente(VentaDetalleModel venta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("CLIENTE");
            sb.AppendLine(string.IsNullOrWhiteSpace(venta.ClienteNombre) ? "  Consumidor final" : $"  {venta.ClienteNombre}");
            if (!string.IsNullOrWhiteSpace(venta.ClienteDocumento)) sb.AppendLine($"  Doc: {venta.ClienteDocumento}");
            if (!string.IsNullOrWhiteSpace(venta.ClienteDireccion)) sb.AppendLine($"  Dir: {venta.ClienteDireccion}");
            if (!string.IsNullOrWhiteSpace(venta.ClienteTelefono)) sb.AppendLine($"  Tel: {venta.ClienteTelefono}");
            sb.AppendLine(Separador);
            return sb.ToString();
        }

        private static string Cuerpo(VentaDetalleModel venta)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{"CANT",-4}{"DESCRIPCION",-18}{"P.U.",9}{"IMPORTE",11}");
            sb.AppendLine(Separador);

            foreach (var comanda in venta.Comandas)
            {
                foreach (var item in comanda.Items)
                {
                    sb.AppendLine($"{item.Cantidad,-4}{Cortar(item.NombreProducto, 18),-18}{item.PrecioUnitario,9:N2}{item.Subtotal,11:N2}");
                    if (!string.IsNullOrWhiteSpace(item.Termino))
                        sb.AppendLine($"    * {Cortar(item.Termino!, 36)}");
                }
            }

            if (!venta.Comandas.SelectMany(c => c.Items).Any())
                sb.AppendLine("(sin productos)");

            sb.AppendLine(Separador);
            return sb.ToString();
        }

        private static string Totales(VentaDetalleModel venta)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{"Subtotal",30}{venta.Subtotal,12:N2}");
            if (venta.Descuento > 0)
                sb.AppendLine($"{"Descuento",30}{-venta.Descuento,12:N2}");
            if (venta.IVA > 0)
                sb.AppendLine($"{"IVA",30}{venta.IVA,12:N2}");
            sb.AppendLine(new string('=', Ancho));
            sb.AppendLine($"{"TOTAL",30}{venta.TotalAmount,12:N2}");
            sb.AppendLine(new string('=', Ancho));
            return sb.ToString();
        }

        private static string Pagos(VentaDetalleModel venta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("PAGOS");
            foreach (var pago in LineasPago(venta))
                sb.AppendLine($"  {pago.Linea,-30}{pago.Monto,12:N2}");

            var vuelto = venta.TotalPagado - venta.TotalAmount;
            if (vuelto > 0)
                sb.AppendLine($"{"VUELTO",30}{vuelto,12:N2}");
            sb.AppendLine(Separador);
            return sb.ToString();
        }

        private static IEnumerable<(string Linea, decimal Monto)> LineasPago(VentaDetalleModel venta) =>
            venta.Pagos.Select(p =>
            {
                var metodo = p.Metodo;
                if (!string.IsNullOrWhiteSpace(p.MarcaTarjeta))
                    metodo = $"{p.MarcaTarjeta} {p.Referencia}";
                return (Linea: metodo, Monto: p.Monto);
            });

        private static string Pie(VentaDetalleModel venta, string referencia)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Centrar("¡GRACIAS POR SU VISITA!"));
            sb.AppendLine(Centrar(referencia));
            sb.AppendLine(Centrar($"{DateTime.Now:dd/MM/yyyy HH:mm}"));
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }

        private static string Centrar(string texto) =>
            texto.Length >= Ancho ? texto : new string(' ', (Ancho - texto.Length) / 2) + texto;

        private static string Cortar(string texto, int max) =>
            texto.Length <= max ? texto : texto.Substring(0, max - 3) + "...";
    }
}
