using System.Drawing;

namespace TrainReservation
{
    public class Tren
    {
        public Tren(string ad, List<Vagon> vagonlar)
        {
            Ad = ad ?? throw new ArgumentNullException(nameof(ad));
            Vagonlar = vagonlar ?? throw new ArgumentNullException(nameof(vagonlar));
        }

        public Tren()
        {
            Ad = String.Empty;
            Vagonlar = new List<Vagon>();
        }

        public string Ad { get; set; }
        public List<Vagon> Vagonlar { get; set; }

        public class Vagon
        {
            public Vagon(string ad, int kapasite, int doluKoltukAdet)
            {
                Ad = ad ?? throw new ArgumentNullException(nameof(ad));
                Kapasite = kapasite;
                DoluKoltukAdet = doluKoltukAdet;
            }

            public string Ad { get; set; }
            public int Kapasite { get; set; }
            public int DoluKoltukAdet { get; set; }
        }
    }
}
