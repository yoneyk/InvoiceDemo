using Starcounter;

partial class InvoicePage : Page, IBound<Invoice> {
    void Handle(Input.AddRow action) {
        new InvoiceRow() {
            Invoice = Data
        };
    }

    void Handle(Input.Save action) {
        if (InvoiceNo == 0) { // A new invoice. 
            InvoiceNo = (int)Db.SQL<long>(
              "SELECT max(i.InvoiceNo) FROM Invoice i").First + 1;
        }
        Transaction.Commit();
        ((InvoicesPage)this.Parent).Invoices = Db.SQL(
          "SELECT i FROM Invoice i"); //refresh invoices list
        ((InvoicesPage)this.Parent).RedirectUrl = "/invoicedemo/invoice/" + InvoiceNo; //redirect to the new URL
    }

    void Handle(Input.Cancel action) {
        bool isUnsavedInvoice = (InvoiceNo == 0);
        Transaction.Rollback();
        if (isUnsavedInvoice) {
            Data = new Invoice();
        }
    }

    void Handle(Input.Delete action) {
        if (Data == null) // Nothing to delete.
            return;

        foreach (var row in Data.Items) {
            row.Delete();
        }
        Data.Delete();
        Transaction.Commit();
        ((InvoicesPage)this.Parent).Invoices = Db.SQL(
          "SELECT i FROM Invoice i"); //refresh invoices list
        ((InvoicesPage)this.Parent).RedirectUrl = "/"; //redirect to the home URL
    }
}
