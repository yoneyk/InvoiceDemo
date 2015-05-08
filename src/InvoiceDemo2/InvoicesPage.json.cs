using Starcounter;

partial class InvoicesPage : Page {
    public void RefreshData() {
        Invoices = Db.SQL("SELECT i FROM Invoice i");
    }
}
