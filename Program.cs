using System;
using Starcounter;
using PolyjuiceNamespace;

class Program {
    static void Main() {
        Handle.GET("/invoicedemo", () => {
            InvoicesPage app;
            if (Session.Current != null) {
                app = (InvoicesPage)Session.Current.Data;
            } else {
                app = new InvoicesPage() {
                    Html = "/InvoicesPage.html"
                };
                app.Session = new Session(SessionOptions.PatchVersioning);
            }

            app.Invoices = Db.SQL("SELECT i FROM Invoice i");
            app.Invoice = Db.Scope<InvoicePage>(() => {
                return new InvoicePage() {
                    Html = "/InvoicePage.html",
                    Data = new Invoice()
                };
            });

            return app;
        });

        Handle.GET("/invoicedemo/invoices/{?}", (int InvoiceNo) => {
            InvoicesPage app = Self.GET<InvoicesPage>("/invoicedemo");
            app.Invoice = Db.Scope<InvoicePage>(() => {
                return new InvoicePage() {
                    Html = "/InvoicePage.html",
                    Data = Db.SQL<Invoice>("SELECT i FROM Invoice i WHERE InvoiceNo = ?", InvoiceNo).First
                };
            });
            return app;
        });

        Polyjuice.Map("/invoicedemo", "/");
    }
}
