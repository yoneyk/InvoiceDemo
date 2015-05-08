using System;
using Starcounter;

class Program {
    static void Main() {
        Handle.GET("/invoicedemo", () => {
            MasterPage master;
            if (Session.Current != null) {
                master = (MasterPage)Session.Current.Data;
            } else {
                master = new MasterPage() {
                    Html = "/MasterPage.html"
                };
                master.Session = new Session(SessionOptions.PatchVersioning);
                master.RecentInvoices = new InvoicesPage() {
                    Html = "/InvoicesPage.html"
                };
            }

            ((InvoicesPage)master.RecentInvoices).RefreshData();
            master.FocusedInvoice = null;
            return master;
        });

        Handle.GET("/invoicedemo/invoices/{?}", (int InvoiceNo) => {
            MasterPage master = X.GET<MasterPage>("/invoicedemo");
            master.FocusedInvoice = Db.Scope<InvoicePage>(() => {
                var page = new InvoicePage() {
                    Html = "/InvoicePage.html",
                    Data = Db.SQL<Invoice>("SELECT i FROM Invoice i WHERE InvoiceNo = ?", InvoiceNo).First
                };
                page.OnSave = ((InvoicesPage)master.RecentInvoices).RefreshData;
                page.OnDelete = ((InvoicesPage)master.RecentInvoices).RefreshData;
                return page;
            });
            return master;
        });

        Handle.GET("/invoicedemo/new-invoice", () => {
            MasterPage master = X.GET<MasterPage>("/invoicedemo");
            master.FocusedInvoice = Db.Scope<InvoicePage>(() => {
                var page = new InvoicePage() {
                    Html = "/InvoicePage.html",
                    Data = new Invoice()
                };
                page.OnSave = ((InvoicesPage)master.RecentInvoices).RefreshData;
                page.OnDelete = ((InvoicesPage)master.RecentInvoices).RefreshData;
                return page;
            });
            return master;
        });
    }
}
