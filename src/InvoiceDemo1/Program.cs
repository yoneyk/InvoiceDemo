using System;
using Starcounter;

class Program {
    static void Main() {
        Handle.GET("/invoicedemo", () => {
            InvoicePage page = Db.Scope<InvoicePage>(() => {
                return new InvoicePage() {
                    Html = "/InvoicePage.html",
                    Data = new Invoice()
                };
            });
            page.Session = new Session(SessionOptions.PatchVersioning);
            return page;
        });

        PolyjuiceNamespace.Polyjuice.Map("/invoicedemo", "/");
    }
}
