using Starcounter;

[Database]
public class Invoice {
    public int InvoiceNo;
    public string Name;
    public decimal Total {
        get {
            return Db.SQL<decimal>("SELECT sum(o.Total) FROM InvoiceRow o WHERE o.Invoice=?", this).First;
        }
    }
    public QueryResultRows<InvoiceRow> Items {
        get {
            return Db.SQL<InvoiceRow>("SELECT o FROM InvoiceRow o WHERE o.Invoice=?", this);
        }
    }
}

[Database]
public class InvoiceRow {
    public Invoice Invoice;
    public string Description;
    public int Quantity;
    public decimal Price;
    public decimal Total {
        get {
            return Quantity * Price;
        }
    }
}
