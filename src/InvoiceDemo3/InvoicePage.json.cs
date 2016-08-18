using Starcounter;
using System;

partial class InvoicePage : Page, IBound<Invoice> {
    public event EventHandler Saved;
    public event EventHandler Deleted;

    void Handle(Input.AddRow action) {
        new InvoiceRow() {
            Invoice = Data
        };
    }

    void Handle(Input.Save action) {
        bool isUnsavedInvoice = (InvoiceNo == 0); // A new invoice
        if (isUnsavedInvoice) {
            InvoiceNo = (int)Db.SQL<long>(
              "SELECT max(i.InvoiceNo) FROM Invoice i").First + 1;
        }
        Transaction.Commit();
        OnSaved();
        RedirectUrl = "/invoicedemo/invoices/" + InvoiceNo; //redirect to the new URL
    }

    void Handle(Input.Cancel action) {
        bool isUnsavedInvoice = (InvoiceNo == 0); // A new invoice
        Transaction.Rollback();
        if (isUnsavedInvoice) {
            Data = new Invoice();
        }
    }

    void Handle(Input.Delete action) {
        // Nothing to delete.
        if (Data == null) {
            return;
        }

        Invoice invoice = this.Data;

        // We have to clean reference from view-model to database object manually before this bug is fixed: https://github.com/Starcounter/Starcounter/issues/2814
        this.Data = null;

        foreach (var row in invoice.Items) {
            row.Delete();
        }

        invoice.Delete();
        Transaction.Commit();
        OnDeleted();
        RedirectUrl = "/invoicedemo"; //redirect to the home URL		
    }

    void Handle(Input.Name action) {
        this.FoundSomebodies.Clear();

        if (!string.IsNullOrEmpty(action.Value)) {
            this.FoundSomebodies = Db.SQL("SELECT s FROM Simplified.Ring1.Somebody s WHERE s.Name LIKE ? ORDER BY s.Name FETCH ?", "%" + action.Value + "%", 8);
        }
    }

    void Handle(Input.ClearFoundSomebodies action) {
        this.FoundSomebodies.Clear();
    }

    protected void OnSaved() {
        if (this.Saved != null) {
            this.Saved(this, EventArgs.Empty);
        }
    }

    protected void OnDeleted() {
        if (this.Deleted != null) {
            this.Deleted(this, EventArgs.Empty);
        }
    }

    [InvoicePage_json.FoundSomebodies]
    public partial class InvoicePageFoundSomebodies : Json, IBound<Simplified.Ring1.Somebody> {
        void Handle(Input.Choose action) {
            this.ParentPage.Name = this.Name;
            this.ParentPage.FoundSomebodies.Clear();
        }

        InvoicePage ParentPage {
            get {
                return this.Parent.Parent as InvoicePage;
            }
        }
    }

    [InvoicePage_json.Items]
    public partial class InvoicePageItems : Json, IBound<InvoiceRow> {
        void Handle(Input.Description action) {
            this.FoundProducts.Clear();

            if (!string.IsNullOrEmpty(action.Value)) {
                this.FoundProducts = Db.SQL("SELECT p FROM Simplified.Ring3.Product p WHERE p.Name LIKE ? ORDER BY p.Name FETCH ?", "%" + action.Value + "%", 8);
            }
        }

        void Handle(Input.ClearFoundProducts action) {
            this.FoundProducts.Clear();
        }
    }

    [InvoicePage_json.Items.FoundProducts]
    public partial class InvoicePageItemsFoundProducts : Json, IBound<Simplified.Ring3.Product> {
        void Handle(Input.Choose action) {
            this.ParentPage.Description = this.Name;
            this.ParentPage.FoundProducts.Clear();
        }

        InvoicePageItems ParentPage {
            get {
                return this.Parent.Parent as InvoicePageItems;
            }
        }
    }
}
