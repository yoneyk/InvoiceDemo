using System;
using Starcounter;

class Program {
    static void Main() {
        Handle.GET("/invoicedemo", () => {
            InvoicePage page;
            if (Session.Current != null) {
                page = (InvoicePage)Session.Current.Data;
            } else {
                page = Db.Scope<InvoicePage>(() => {
                    return new InvoicePage() {
                        Html = "/InvoiceDemo/InvoicePage.html",
                        Data = new Invoice()
                    };
                });
                page.Session = new Session(SessionOptions.PatchVersioning);
            }
            return page;
        });
    }
}
