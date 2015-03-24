using Starcounter;

[Database]
public class Invoice {
    public int InvoiceNo;
    public string Name;
    public decimal Total {
        get {
            return Db.SQL<decimal>("SELECT sum(r.Total) FROM InvoiceRow r WHERE r.Invoice=?", this).First;
        }
    }
    public QueryResultRows<InvoiceRow> Items {
        get {
            return Db.SQL<InvoiceRow>("SELECT r FROM InvoiceRow r WHERE r.Invoice=?", this);
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
