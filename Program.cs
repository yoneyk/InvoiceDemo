using System;
using Starcounter;

class Program {
    static void Main() {
        Handle.GET("/invoicedemo/master", () => {
            if (Session.Current != null) {
                return Session.Current.Data;
            }

            var app = new Master() {
                Html = "/Master.html"
            };

            app.Session = new Session(SessionOptions.PatchVersioning);

            return app;
        });

        Handle.GET("/invoicedemo", () => {
            Master master = X.GET<Master>("/invoicedemo/master");
            
            InvoicesPage app;
            if (master.FocusedPage is InvoicesPage) {
                app = (InvoicesPage)master.FocusedPage;
            } 
            else {
                app = new InvoicesPage() {
                    Html = "/InvoicesPage.html"
                };
                master.FocusedPage = app;
            }

            app.Invoices = Db.SQL("SELECT i FROM Invoice i");
            app.Invoice = Db.Scope<InvoicePage>(() => {
                return new InvoicePage() {
                    Html = "/InvoicePage.html",
                    Data = new Invoice()
                };
            });

            return master;
        });

        Handle.GET("/invoicedemo/invoices/{?}", (int InvoiceNo) => {
            Master master = X.GET<Master>("/invoicedemo");
            InvoicesPage app = (InvoicesPage)master.FocusedPage;
            app.Invoice = Db.Scope<InvoicePage>(() => {
                return new InvoicePage() {
                    Html = "/InvoicePage.html",
                    Data = Db.SQL<Invoice>("SELECT i FROM Invoice i WHERE InvoiceNo = ?", InvoiceNo).First
                };
            });
            return master;
        });

        PolyjuiceNamespace.Polyjuice.Map("/invoicedemo", "/");
    }
}
