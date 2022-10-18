using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace TrainReservation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TrenController : ControllerBase
    {
        public int _maxSeatCount;

        private static List<Tren> _trenList = new()
        {
            new Tren("Başkent Ekspres", new List<Tren.Vagon>{new Tren.Vagon("Vagon 1", 100, 68), new Tren.Vagon("Vagon 2", 90, 50), new Tren.Vagon("Vagon 3", 80, 80)}),
            new Tren("Doğu Ekspres", new List<Tren.Vagon>{new Tren.Vagon("Vagon 1", 200, 120), new Tren.Vagon("Vagon 2", 160, 100), new Tren.Vagon("Vagon 3", 140, 70)})
        };

        private readonly ILogger<TrenController> _logger;

        public TrenController(ILogger<TrenController> logger)
        {
            _logger = logger;
        }

        [NonAction]
        public void onRequest()
        {
            _logger.LogInformation("Sayfa şu tarihte edildi: {DT}",
                DateTime.UtcNow.ToLongTimeString());
        }

        [HttpPost]
        public Bilet IsAvailable(string jsonRequest)
        {
            Bilet response = new()
            {
                YerlesimAyrinti = new List<Bilet.TicketVagon>()
            };

            try
            {
                Rezervasyon request = JsonSerializer.Deserialize<Rezervasyon>(jsonRequest)
                    ?? throw new ArgumentNullException(nameof(jsonRequest));
                Tren requestedTrain = _trenList.First(x => x.Ad == request.Tren.Ad)
                    ?? throw new ArgumentNullException(nameof(request.Tren.Ad), "Aranan Tren Bulunamadı.");

                foreach (var item in requestedTrain.Vagonlar)
                {
                    _maxSeatCount = item.Kapasite * 70 / 100;
                    int availableSeats = _maxSeatCount - item.DoluKoltukAdet;
                    if (availableSeats < (request.KisilerFarkliVagonlaraYerlestirilebilir ? -1 : request.RezervasyonYapilacakKisiSayisi))
                    {
                        continue;
                    }

                    if (request.KisilerFarkliVagonlaraYerlestirilebilir)
                    {
                        Bilet.TicketVagon ticketVagon = new() { VagonAdi = item.Ad, KisiSayisi = 0 };
                        do //vagonda alabileceği maksimum koltuk sayısını alana kadar veya rezerve işlemini tamamlayana kadar devam eder.
                        {
                            ticketVagon.KisiSayisi++;
                            availableSeats--;
                            request.RezervasyonYapilacakKisiSayisi--;
                        } while (availableSeats > 0 && request.RezervasyonYapilacakKisiSayisi > 0);
                        response.YerlesimAyrinti.Add(ticketVagon);
                        if (request.RezervasyonYapilacakKisiSayisi <= 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        response.YerlesimAyrinti.Add(new Bilet.TicketVagon { VagonAdi = item.Ad, KisiSayisi = request.RezervasyonYapilacakKisiSayisi });
                        break;
                    }
                }
                if (request.RezervasyonYapilacakKisiSayisi > 0) //rezerve isteğinin tamamını veremediği için işlemi hatayla sonlandırır.
                {
                    throw new Exception();
                }

                response.RezervasyonYapilabilir = true;
                return response;

            }
            catch (Exception)
            {
                return new Bilet { RezervasyonYapilabilir = false, YerlesimAyrinti = new List<Bilet.TicketVagon>() };
            }

        }
    }
}
