namespace TrainReservation
{
    public class Bilet
    {
        public bool RezervasyonYapilabilir { get; set; }
        public List<TicketVagon> YerlesimAyrinti { get; set; }
        public class TicketVagon
        {
            public string VagonAdi { get; set; }
            public int KisiSayisi { get; set; }
        }
    }
}
