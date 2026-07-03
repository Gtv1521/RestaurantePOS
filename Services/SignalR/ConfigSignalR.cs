using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace MiComanderaApp.Services.SignalR
{

    public class SignalRService
    {
        // Instancia única (Patrón Singleton)
        private static SignalRService? _instance;
        public static SignalRService Instance => _instance ??= new SignalRService();

        private HubConnection _connection = null!;

        // EVENTOS: Los formularios se suscribirán a estos eventos
        public event Action<string>? OnUsuarioCreado;
        public event Action<string>? OnEstadoConexionCambiado;

        // Constructor privado para el Singleton
        private SignalRService()
        {
            ConfigurarConexion();
        }

        private void ConfigurarConexion()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{AppConfig.ApiSettings.BaseUrl}{AppConfig.ApiSettings.Endpoints.RestaurantHub}")
                .WithAutomaticReconnect()
                .Build();

            // Escuchar el método del servidor y disparar el evento de C#
            _connection.On<string>("UsuarioCreado", (nombre) =>
            {
                // Dispara el evento si hay algún formulario escuchando
                OnUsuarioCreado?.Invoke(nombre);
            });

            // Monitorear estado de la conexión
            _connection.Closed += async (error) => OnEstadoConexionCambiado?.Invoke("Desconectado");
            _connection.Reconnecting += async (error) => OnEstadoConexionCambiado?.Invoke("Reconectando...");
            _connection.Reconnected += async (connectionId) => OnEstadoConexionCambiado?.Invoke("Conectado");
        }

        // Método para iniciar la conexión (se llama una sola vez al abrir la app)
        public async Task IniciarAsync()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _connection.StartAsync();
                    OnEstadoConexionCambiado?.Invoke("Conectado");
                }
                catch (Exception)
                {
                    OnEstadoConexionCambiado?.Invoke("Error al conectar");
                }
            }
        }

        // Método para enviar datos a la API desde cualquier módulo
        public async Task EnviarMensajeAsync(string metodo, object datos)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync(metodo, datos);
            }
        }
    }

}