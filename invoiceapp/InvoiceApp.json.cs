using Starcounter;
using System;
using System.Collections;

[InvoiceApp_json]
partial class InvoiceApp : Json {
    static void Main() {
        Starcounter.Handle.GET("/", (Request r) => {
            var invoiceApp = new InvoiceApp() {
                Html = "/invoiceapp.html"
            };
            invoiceApp.Session = new Session();
            invoiceApp.Transaction = new Transaction();
            invoiceApp.Transaction.Add(() => {
                invoiceApp.Invoices = SQL("SELECT I FROM Invoice I");
            });
            return invoiceApp;
        });

        #region Sample Data
        Starcounter.Handle.GET("/init", (Request r) => {
            SampleData();
            return System.Net.HttpStatusCode.Created;
        });
        #endregion
    }

    void Handle(Input.Add action) {
        this.Invoice.Data = new Invoice() { InvoiceNo = 0 };
    }

    [InvoiceApp_json.Invoices]
    partial class InvoiceListElementJson : Json {
        void Handle(Input.Show show) {
            Invoice invoice = this.Data as Invoice;
            var inv = ((InvoiceApp)this.Parent.Parent).Invoice;
            inv.Data = invoice;
        }
    }

    [InvoiceApp_json.Invoice]
    partial class InvoiceJson : Json {
        void Handle(Input.Save action) {

            var invoice = (Invoice)this.Data;
            invoice.InvoiceNo = Db.SQL<int>("SELECT max(o.InvoiceNo) FROM Invoice o").First + 1;

            this.Transaction.Commit();
        }
        void Handle(Input.Cancel action) {
            this.Transaction.Rollback();
        }
        void Handler(Input.Delete action) {
            this.Data.Delete();
        }
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