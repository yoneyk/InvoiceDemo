using Starcounter;

partial class InvoicesPage : Page {
    public void RefreshData() {
        Invoices = Db.SQL("SELECT i FROM Invoice i");
    }

    [InvoicesPage_json.Invoices]
    partial class InvoicesItemPage {
        protected override void OnData() {
            base.OnData();
            this.Url = string.Format("/invoicedemo/invoices/{0}", this.InvoiceNo);
        }
    }
}
