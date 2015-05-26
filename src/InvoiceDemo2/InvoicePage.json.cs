using Starcounter;
using System;

partial class InvoicePage : Page, IBound<Invoice> {
    public Action OnSave = () => { };
    public Action OnDelete = () => { };

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
        OnSave();
        RedirectUrl = "/invoicedemo/invoices/" + InvoiceNo; //redirect to the new URL
    }

    void Handle(Input.Cancel action) {
        bool isUnsavedInvoice = (InvoiceNo == 0); // A new invoice
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
        OnDelete();
        RedirectUrl = "/invoicedemo"; //redirect to the home URL		
    }
}
