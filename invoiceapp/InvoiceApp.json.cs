using Starcounter;
using System;
using System.Collections;

[InvoiceApp_json]
partial class InvoiceApp : Json {
    static void Main() {
        Starcounter.Handle.GET("/", (Request r) => {
            if (Session.Current != null)
                return Session.Current.Data;

            var invoiceApp = new InvoiceApp() {
                Html = "/invoiceapp.html"
            };
            invoiceApp.Session = new Session();
            invoiceApp.Invoices = SQL("SELECT I FROM Invoice I");
            invoiceApp.Invoice = new InvoicePage();
            invoiceApp.Invoice.Transaction = new Transaction(() => {
                invoiceApp.Invoice.Data = new Invoice() { InvoiceNo = 0 };
            });
            return invoiceApp;
        });

        Starcounter.Handle.GET("/invoice/{?}", (int InvoiceNo, Request r) =>
        {
            InvoiceApp invoiceApp = InvoiceApp.GET("/");
            invoiceApp.Invoice = new InvoicePage();
            invoiceApp.Invoice.Transaction = new Transaction(() => {
                invoiceApp.Invoice.Data = Db.SQL("SELECT I FROM Invoice I WHERE InvoiceNo = ?", InvoiceNo).First;
            });
            return invoiceApp.Invoice;
        });

        #region Sample Data
        Starcounter.Handle.GET("/init", (Request r) => {
            SampleData();
            return System.Net.HttpStatusCode.Created;
        });
        #endregion
    }

    void Handle(Input.Add action) {
        Invoice = new InvoicePage();
        Invoice.Transaction = new Transaction(() => {
            Invoice.Data = new Invoice() { InvoiceNo = 0 };
        });
    }

    [InvoiceApp_json.Invoices]
    partial class InvoiceListElementJson : Json {
    }

    // Browsers will ask for "text/html" and we will give it to them
    // by loading the contents of the URI in our Html property
    public override string AsMimeType(MimeType type) {
        if (type == MimeType.Text_Html) {
            return X.GET<string>(Html);
        }
        return base.AsMimeType(type);
    }

    #region SampleDate
    static void SampleData() {

        Db.Transaction(() => {
            Db.SlowSQL("DELETE FROM Invoice");
            Db.SlowSQL("DELETE FROM InvoiceRow");
        });

        Db.Transaction(() => {
            Invoice a = new Invoice() { InvoiceNo = 1, Name = "Starcounter AB" };
            new InvoiceRow() { Invoice = a, Description = "Coca-Cola", Quantity = 2, Price = 10.0m };
            new InvoiceRow() { Invoice = a, Description = "Big-Mac", Quantity = 1, Price = 45.0m };

            Invoice b = new Invoice() { InvoiceNo = 2, Name = "Tunity AB" };
            new InvoiceRow() { Invoice = b, Description = "Saab", Quantity = 20, Price = 10m };
            new InvoiceRow() { Invoice = b, Description = "BMW", Quantity = 3, Price = 450m };

            Invoice c = new Invoice() { InvoiceNo = 3, Name = "Heads AB" };
            new InvoiceRow() { Invoice = c, Description = "Apple", Quantity = 1, Price = 900m };
            new InvoiceRow() { Invoice = c, Description = "IBM", Quantity = 1, Price = 3000m };
        });
    }
    #endregion
}

[Database]
public class Invoice {
    public int InvoiceNo;
    public string Name;
    public decimal Total { get { return Db.SQL<decimal>("SELECT sum(o.Total) FROM InvoiceRow o WHERE o.Invoice=?", this).First; } }
    public IEnumerable Items { get { return Db.SQL<InvoiceRow>("SELECT o FROM InvoiceRow o WHERE o.Invoice=?", this); } }
}

[Database]
public class InvoiceRow {
    public Invoice Invoice;
    public string Description;
    public int Quantity;
    public decimal Price;
    public decimal Total { get { return Quantity * Price; } }
}
