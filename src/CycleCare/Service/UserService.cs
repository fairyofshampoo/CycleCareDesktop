﻿using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Text;
using CycleCare.Models;
using CycleCare.Utilities;

namespace CycleCare.Service
{
    public class UserService
    {
        private static readonly string TOKEN = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["TOKEN"].Value;
        private static readonly string URL = string.Concat(Properties.Resources.BASE_URL, "users/");

        public static async Task<Response> Login(User user)
        {
            Response response = new Response();
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var httpRequestMessage = new HttpRequestMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(string.Concat(URL, "login"))
                    };

                    HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                    if (httpResponseMessage != null)
                    {
                        response.Code = (int)httpResponseMessage.StatusCode;

                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            string json = await httpResponseMessage.Content.ReadAsStringAsync();
                            response = JsonConvert.DeserializeObject<Response>(json);
                        }
                    }
                    else
                    {
                        response.Code = (int)HttpStatusCode.InternalServerError;
                        response.Details = "No se recibió respuesta del servidor.";
                    }
                }
                catch (HttpRequestException ex)
                {
                    response.Code = (int)HttpStatusCode.InternalServerError;
                    response.Details = $"Error de red: {ex.Message}";
                    DialogManager.ShowErrorMessageBox(response.Details);
                }
                catch (JsonException ex)
                {
                    response.Code = (int)HttpStatusCode.InternalServerError;
                    response.Details = $"Error al procesar la respuesta JSON: {ex.Message}";
                    DialogManager.ShowErrorMessageBox(response.Details);
                }
                catch (Exception ex)
                {
                    response.Code = (int)HttpStatusCode.InternalServerError;
                    response.Details = $"Error inesperado: {ex.Message}";
                    DialogManager.ShowErrorMessageBox(response.Details);
                }
            }
            return response;
        }

    }
}
