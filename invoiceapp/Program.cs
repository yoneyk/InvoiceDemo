using System;
using Starcounter;

class Program {
    static void Main() {
        Handle.GET("/master", () => {
            if (Session.Current != null) {
                return Session.Current.Data;
            }

            var app = new Master() {
                Html = "/master.html"
            };

            app.Session = new Session(SessionOptions.PatchVersioning);

            return app;
        });

        Handle.GET("/", () => {
            Master master = Master.GET("/master");
            
            InvoiceApp app;
            if (master.FocusedPage is InvoiceApp) {
                app = (InvoiceApp)master.FocusedPage;
            } 
            else {
                app = new InvoiceApp() {
                    Html = "/invoiceapp.html"
                };
                master.FocusedPage = app;
            }

            app.Invoices = Db.SQL("SELECT i FROM Invoice i");

            app.Invoice = new InvoicePage() {
                Html = "/invoicepage-add.html"
            };
            app.Invoice.Transaction = new Transaction(() => {
                app.Invoice.Data = new Invoice();
            });

            return app;
        });

        Handle.GET("/invoice/{?}", (int InvoiceNo) => {
            InvoiceApp app = InvoiceApp.GET("/");
            app.Invoice = new InvoicePage() {
                Html = "/invoicepage.html"
            };
            app.Invoice.Transaction = new Transaction(() => {
                app.Invoice.Data = Db.SQL("SELECT i FROM Invoice i WHERE InvoiceNo = ?", InvoiceNo).First;
            });
            return app.Invoice;
        });
    }
}
