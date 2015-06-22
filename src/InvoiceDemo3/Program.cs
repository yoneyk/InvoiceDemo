using System;
using Starcounter;

class Program {
    static void Main() {
        Handle.GET("/invoicedemo", () => {
            MasterPage master;

            if (Session.Current != null && Session.Current.Data != null) {
                master = (MasterPage)Session.Current.Data;
            } else {
                master = new MasterPage();

                if (Session.Current != null) {
                    master.Html = "/InvoiceDemo/LauncherWrapperPage.html";
                    master.Session = Session.Current;
                } else {
                    master.Session = new Session(SessionOptions.PatchVersioning);
                }

                master.RecentInvoices = new InvoicesPage() {
                    Html = "/InvoiceDemo/InvoicesPage.html"
                };
            }

            ((InvoicesPage)master.RecentInvoices).RefreshData();
            master.FocusedInvoice = null;

            return master;
        });

        Handle.GET("/invoicedemo/invoices/{?}", (int InvoiceNo) => {
            MasterPage master = Self.GET<MasterPage>("/invoicedemo");
            master.FocusedInvoice = Db.Scope<InvoicePage>(() => {
                var page = new InvoicePage() {
                    Html = "/InvoiceDemo/InvoicePage.html",
                    Data = Db.SQL<Invoice>("SELECT i FROM Invoice i WHERE InvoiceNo = ?", InvoiceNo).First
                };

                page.Saved += (s, a) => {
                    ((InvoicesPage)master.RecentInvoices).RefreshData();
                };

                page.Deleted += (s, a) => {
                    ((InvoicesPage)master.RecentInvoices).RefreshData();
                };

                return page;
            });
            return master;
        });

        Handle.GET("/invoicedemo/new-invoice", () => {
            MasterPage master = Self.GET<MasterPage>("/invoicedemo");
            master.FocusedInvoice = Db.Scope<InvoicePage>(() => {
                var page = new InvoicePage() {
                    Html = "/InvoiceDemo/InvoicePage.html",
                    Data = new Invoice()
                };

                page.Saved += (s, a) => {
                    ((InvoicesPage)master.RecentInvoices).RefreshData();
                };

                page.Deleted += (s, a) => {
                    ((InvoicesPage)master.RecentInvoices).RefreshData();
                };
                
                return page;
            });
            return master;
        });

        Handle.GET("/invoicedemo/app-name", () => {
            return new AppName();
        });

        Handle.GET("/invoicedemo/app-icon", () => {
            return new Page() { Html = "/InvoiceDemo/AppIconPage.html" };
        });

        Handle.GET("/invoicedemo/menu", () => {
            return new Page() { Html = "/InvoiceDemo/AppMenuPage.html" };
        });

        PolyjuiceNamespace.Polyjuice.Map("/invoicedemo/menu", "/polyjuice/menu");
        PolyjuiceNamespace.Polyjuice.Map("/invoicedemo/app-name", "/polyjuice/app-name");
        PolyjuiceNamespace.Polyjuice.Map("/invoicedemo/app-icon", "/polyjuice/app-icon");
    }
}
