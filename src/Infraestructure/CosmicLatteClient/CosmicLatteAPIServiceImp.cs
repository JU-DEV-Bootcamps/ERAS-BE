using Entities;
using Microsoft.Extensions.Configuration;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CosmicLatteClient
{
    public class CosmicLatteAPIServiceImp : ICosmicLatteAPIService<CosmicLatteStatus>
    {
        // private List<Entities.Evaluation> _evals;
        private const string _PATH_EVALUATION = "evaluations";
        private const string _HEADER_API_KEY = "x-apikey";
        private string _API_KEY;
        private string _API_URL;

        private readonly HttpClient _httpClient;

        public CosmicLatteAPIServiceImp(IConfiguration configuration, HttpClient httpClient)
        {
            _API_KEY = configuration["CosmicLatte:ApiKey"];
            _API_URL = configuration["CosmicLatte:URL"];
            _httpClient = httpClient;
            // _evals = new List<Entities.Evaluation>();
        }

        private string ConvertStringToIsoExtendedDate(string date)
        {
            string[] parts = date.Split('-');
            int year = int.Parse(parts[0]);
            int month = parts.Length > 1 ? int.Parse(parts[1]) : 1;
            int day = parts.Length > 2 ? int.Parse(parts[2]) : 1;
            DateTime dateFromDate = new DateTime(year, month, day);
            return dateFromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public async Task<CosmicLatteStatus> CosmicApiIsHealthy()
        {
            string path = _API_URL + _PATH_EVALUATION;
            var request = new HttpRequestMessage(HttpMethod.Get, path + "?$filter=contains(name,' ')");
            request.Headers.Add(_HEADER_API_KEY, _API_KEY); // Set auth header

            try
            {
                var response = await _httpClient.SendAsync(request); // verificar si hay alguna forma mas performante para chequear status
                                                                     // todo Logger?
                return new CosmicLatteStatus(response.IsSuccessStatusCode ? true : false); // velocidad o tiempo demora?
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}"); // todo Logger!
                throw new Exception($"Hubo un error con la solicitud");
            }
        }
        /*
        public async Task<ActionResult<Entities.Evaluation>> GetEvaluationById(string idEval)
        {   // ID example NYFx8G8kSaSm64B5e
            // verificar validacion de encuesta.. ej MBNgMXWvuJszn3Sx9 => no esta validada y por ende la api falla, establecer manejo de error
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{idEval}\"}}}}", Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _PATH_EVALUATION + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(_HEADER_API_KEY, _API_KEY); // Set auth header

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ApiResponseModelEvaluation apiResponse = JsonSerializer.Deserialize<ApiResponseModelEvaluation>(responseBody);
                    // todo aca queda pendiente pasar el score.. si es que es necesario, pero requiere un mapeo de respuesta con peso, o otra llamada a la api
                    return MapEval(apiResponse);
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error de servidor Cosmic latte: {e.Message}"); // todo Logger!
                throw new Exception($"Error de servidor Cosmic latte: {e.Message}");
            }
        }
        */
        /*
        public async Task<List<Entities.Evaluation>> GetEvaluations(string name, string startDate, string endDate)
        {
            string path = _API_URL + _PATH_EVALUATION;
            if (name != null || startDate != null || endDate != null)
            {
                // ?$filter=contains(name, 'Encuesta') and startedAt ge 2024-12-24T14:22:47.913Z and startedAt le 2025-12-24T14:23:47.913Z
                // verificar casos, existe la posibilidad que no me manden el nombre? tipo pidiendo TODA LA INFO?
                path += "?$filter=";
                if (name != null) path += $"contains(name,'{name}')"; // Chequear: SI paso un solo caracter "E", no me toma la api, pero si mando "En" si.. Por?
                if (startDate != null) path += $" and startedAt ge {ConvertStringToIsoExtendedDate(startDate)}";
                if (endDate != null) path += $" and startedAt le {ConvertStringToIsoExtendedDate(endDate)}";
            }
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(_HEADER_API_KEY, _API_KEY); // Set auth header

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ApiResponseModelAllEvaluations apiResponse = JsonSerializer.Deserialize<ApiResponseModelAllEvaluations>(responseBody);
                    List<DataItem> evaluations = apiResponse.data;
                    evaluations.ForEach((evaluation) => _evals.Add(MapEval(evaluation)));
                    return _evals;
                }
                else
                {
                    // if (response.StatusCode == HttpStatusCode.NotFound) return new List<Entities.Evaluation>();
                    throw new Exception($"Error NO IsSuccessStatusCode: {response.StatusCode} - {response.ReasonPhrase}. Respuesta: {await response.Content.ReadAsStringAsync()}");  // todo Logger!
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error *************************************: {e.Message}");  // todo Logger!
                throw new Exception($"Hubo un error con la solicitud");
            }
        }

        */


        /*
        public Entities.Evaluation MapEval(DataItem evaluation)
        {
            List<ByPosition> listByPosition = evaluation.score is not null ? evaluation.score.byPosition : new List<ByPosition>();
            // todo verificar validacion de encuesta.. ej MBNgMXWvuJszn3Sx9 => no esta validada
            // todo varios campos fallan, ej score, deberiamos preguntar si debemos tomarla en cuenta o no
            // todo al no tener las respuestas, entiendo que no tiene sentido almacenarla siquiera

            Dictionary<int, Answers> AllAnswersFromEval = new Dictionary<int, Answers>();
            for (int i = 0; i < listByPosition.Count; i++)
            {
                Answers ans = new Answers();
                Question question = new Question();
                question.Body = new Dictionary<string, string>();
                question.Body.Add("es", "deberia ser el texto de la preg?");  // todo ans.AnswersList = new string[]; => esto deberia obtenerlo de mi BD para mapearlo por posicion y obtener el texto de la pregunta
                ans.Question = question;
                ans.Position = listByPosition[i].position;
                ans.Type = "sin tipo";
                ans.CustomSettings = null;
                ans.Score = listByPosition[i].score;

                ans.AnswersList = new string[] { "Una respuesta", "Otra respuesta" }; // todo ans.AnswersList => es la lista de respuestas, si fuera necesario, deberia obterlo en otra llamada?
                AllAnswersFromEval.Add(i, ans);
            }
            return new Entities.Evaluation
            {
                Id = evaluation._id,
                StartedAt = evaluation.startedAt,
                CreationDate = DateTime.Now,
                CreationUser = "API",
                AnswersList = AllAnswersFromEval
            };

        }
        */

        /*
        public Entities.Evaluation MapEval(ApiResponseModelEvaluation resp)
        {
            return new Entities.Evaluation
            {
                Id = resp.Data._id,
                StartedAt = resp.Data.Evaluation.StartedAt,
                CreationDate = DateTime.Now,
                CreationUser = "API",
                AnswersList = resp.Data.Answers // todo aca queda pendiente pasar el score.. si es que es necesario, pero requiere un mapeo de respuesta con peso, o otra llamada a la api
            };
        }
        */

    }
}