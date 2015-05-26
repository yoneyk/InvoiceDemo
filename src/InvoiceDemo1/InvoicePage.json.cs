using Starcounter;

partial class InvoicePage : Page, IBound<Invoice> {
    void Handle(Input.AddRow action) {
        new InvoiceRow() {
            Invoice = Data
        };
    }

    void Handle(Input.Save action) {
        bool isUnsavedInvoice = (InvoiceNo == 0); // A new invoice
        if (isUnsavedInvoice) {
            InvoiceNo = (int)Db.SQL<long>(
              "SELECT max(i.InvoiceNo) FROM Invoice i").First + 1;
        }
        Transaction.Commit();
    }

    void Handle(Input.Cancel action) {
        bool isUnsavedInvoice = (InvoiceNo == 0); // A new invoice
        Transaction.Rollback();
        if (isUnsavedInvoice) {
            Data = new Invoice();
        }
    }
}
