using System;
using Starcounter;

class Program {
    static void Main() {
        Handle.GET("/", () => {
            if (Session.Current != null) {
                return Session.Current.Data;
            }

            var app = new InvoiceApp() {
                Html = "/invoiceapp.html"
            };

            app.Invoices = Db.SQL("SELECT i FROM Invoice i");

            app.Invoice = new InvoicePage();
            app.Invoice.Transaction = new Transaction(() => {
                app.Invoice.Data = new Invoice();
            });

            app.Session = new Session(SessionOptions.PatchVersioning);

            return app;
        });

        Handle.GET("/invoice/{?}", (int InvoiceNo) => {
            InvoiceApp app = InvoiceApp.GET("/");
            app.Invoice = new InvoicePage();
            app.Invoice.Transaction = new Transaction(() => {
                app.Invoice.Data = Db.SQL("SELECT i FROM Invoice i WHERE InvoiceNo = ?", InvoiceNo).First;
            });
            return app.Invoice;
        });
    }
}
