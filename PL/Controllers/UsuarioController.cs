using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace PL.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly IConfiguration _configuration;

        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public UsuarioController(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ActionResult Movies()
        {
            ML.Movie movie = new ML.Movie();
            movie.Movies = new List<object>();

            string urlAPI = _configuration["UrlAPI"];
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlAPI);

                var responseTask = client.GetAsync("16835743/favorite/movies?api_key=894c926f3620ddcb0f736f451a70682d&session_id=4a345ee267cb85042d8a7556f3222028811c6166&language=es-ES&sort_by=created_at.asc&page=1");

                responseTask.Wait();

                var resultServicio = responseTask.Result;

                if(resultServicio.IsSuccessStatusCode)
                {
                    var readTask = resultServicio.Content.ReadAsStringAsync();
                    dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                    readTask.Wait();


                    foreach(var resultItem in resultJSON.results)
                    {
                        ML.Movie resultItemList = new ML.Movie();
                        resultItemList.IdMovie = resultItem.id;
                        resultItemList.Titulo = resultItem.title;
                        resultItemList.Descripcion = resultItem.overview;
                        resultItemList.Imagen = "https://www.themoviedb.org/t/p/w1280" +resultItem.poster_path;

                        movie.Movies.Add(resultItemList);
                    }

                }
            }
            return View(movie); 
        }

        [HttpGet]
        public ActionResult Populares()
		{
            ML.Movie movie = new ML.Movie();
            movie.Movies = new List<object>();

            string urlAPI = _configuration["UrlAPIMovie"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlAPI);

                var responseTask = client.GetAsync("popular?api_key=894c926f3620ddcb0f736f451a70682d&language=es-ES&page=1");

                responseTask.Wait();

                var resultServicio = responseTask.Result;

                if (resultServicio.IsSuccessStatusCode)
                {
                    var readTask = resultServicio.Content.ReadAsStringAsync();
                    dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                    readTask.Wait();


                    foreach (var resultItem in resultJSON.results)
                    {
                        ML.Movie resultItemList = new ML.Movie();
                        resultItemList.IdMovie = resultItem.id;
                        resultItemList.Titulo = resultItem.title;
                        resultItemList.Descripcion = resultItem.overview;
                        resultItemList.Imagen = "https://www.themoviedb.org/t/p/w1280" + resultItem.poster_path;

                        movie.Movies.Add(resultItemList);
                    }

                }
            }
            return View(movie);
        }

        [HttpGet]
        public ActionResult Favoritos(int IdMovie, bool favorite)
		{
            ML.Favorites movieFavorite = new ML.Favorites();
            movieFavorite.media_type = "movie";
            movieFavorite.media_id = IdMovie;
            movieFavorite.favorite = favorite;
            ML.Result result = new ML.Result();           
			try
			{
                string urlAPI = _configuration["UrlAPI"];
                using (var client = new HttpClient())
				{
                    client.BaseAddress = new Uri(urlAPI);
                    string adress = "12iwj2uaa/favorite?api_key=894c926f3620ddcb0f736f451a70682d&session_id=4a345ee267cb85042d8a7556f3222028811c6166";
                    var postTask = client.PostAsJsonAsync<ML.Favorites>(adress, movieFavorite);
                    postTask.Wait();

                    var resultServicio = postTask.Result;
					if (resultServicio.IsSuccessStatusCode)
					{
                        result.Correct = true;
					}
					else
					{
                        result.Correct = false;
					}
				}

			}
            catch (Exception ex)
			{
                result.Correct = false;
                result.EX = ex;
			}
			if (result.Correct)
			{
                return Redirect("Movies");
			}
			else
			{
                return Redirect("Populares");
			}
            
		}


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            ML.Result result = BL.Usuario.GetPassword(Email);

            if (result.Correct)
            {
                ML.Usuario usuario = (ML.Usuario)result.Object;

                if(usuario.Password == Password)
                {
                    return Redirect("Home/Index");
                }
                else
                {
                    ViewBag.Message = "El Email o contraseña es incorrecto...";
                    return PartialView("ModalLogin");
                }
            }
            else
            {
                ViewBag.Message = "El Email o contraseña es incorrecto... ";
                return PartialView("ModalLogin");
            }
        }
    }
}
