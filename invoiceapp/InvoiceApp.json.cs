using Starcounter;

partial class InvoiceApp : Page {
    void Handle(Input.Add action) {
        Invoice = new InvoicePage();
        Invoice.Transaction = new Transaction(() => {
            Invoice.Data = new Invoice();
        });
    }
}
