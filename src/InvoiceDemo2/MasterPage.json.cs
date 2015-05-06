using Starcounter;

partial class MasterPage : Page {
    public void RefreshList() {
        InvoicesPage recentInvoices = (InvoicesPage)RecentInvoices;
        recentInvoices.Invoices = Db.SQL("SELECT i FROM Invoice i");
    }
}
