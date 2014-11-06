using Starcounter;
using System;

[InvoicePage_json]
partial class InvoicePage : Json {
    void Handle(Input.Save action) {

        var invoice = (Invoice)this.Data;

        if (invoice == null) // Nothing to save.
            return;

        if (invoice.InvoiceNo == 0) { // A new invoice. 
            invoice.InvoiceNo = (int)Db.SQL<Int64>("SELECT max(o.InvoiceNo) FROM Invoice o").First + 1;
        }

        this.Transaction.Commit();

        ((InvoiceApp)this.Parent).Invoices = SQL("SELECT I FROM Invoice I"); //refresh invoices list
        ((InvoiceApp)this.Parent).RedirectUrl = "/invoice/" + invoice.InvoiceNo; //redirect to the new URL

    }
    void Handle(Input.AddRow action) {
        new InvoiceRow() {
            Invoice = (Invoice)this.Data
        };
    }
    void Handle(Input.Cancel action) {
        bool isUnsavedInvoice = (InvoiceNo == 0);
        this.Transaction.Rollback();
        if (isUnsavedInvoice) {
            Data = new Invoice();
        }
    }
    void Handle(Input.Delete action) {
        var invoice = (Invoice)this.Data;

        if (invoice == null) // Nothing to delete.
            return;

        foreach (var row in invoice.Items) {
            row.Delete();
        }
        invoice.Delete();
        this.Transaction.Commit();
        ((InvoiceApp)this.Parent).Invoices = SQL("SELECT I FROM Invoice I"); //refresh invoices list
        this.Data = new Invoice(); //display fresh invoice after one was deleted
        ((InvoiceApp)this.Parent).RedirectUrl = "/"; //redirect to the home URL
    }
}
