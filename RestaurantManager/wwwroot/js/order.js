function itemState(menuItemId) {
    return {
        qty: 0,
        menuItemId: menuItemId,

        init() {
            this.qty = initialQuantities[String(this.menuItemId)] ?? 0;

            document.addEventListener('basket-updated', (e) => {
                if (e.detail.quantities && e.detail.quantities[this.menuItemId] !== undefined) {
                    this.qty = e.detail.quantities[String(this.menuItemId)];
                }
            });
        },

        async add() {
            this.qty++;
            try {
                const res = await fetch('/order/additem', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: `id=${this.menuItemId}&__RequestVerificationToken=${encodeURIComponent(getToken())}`
                });
                const data = await res.json();

                if (data.error)
                    throw new Error(data.error);

                this.qty = data.quantity;
                document.dispatchEvent(new CustomEvent('basket-updated-local', { detail: { basketCount: data.basketCount } }));
            } catch (err) {
                console.error(err.message);
                if (err.message === 'no_session')
                    window.location.href = '/';
                this.qty--;
            }
        },

        async remove() {
            if (this.qty > 0) {
                this.qty--;
                try {
                    const res = await fetch('/order/removeitem', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        body: `id=${this.menuItemId}&__RequestVerificationToken=${encodeURIComponent(getToken())}`
                    });
                    const data = await res.json();

                    if (data.error)
                        throw new Error(data.error);

                    this.qty = data.quantity;
                    document.dispatchEvent(new CustomEvent('basket-updated-local', { detail: { basketCount: data.basketCount } }));
                } catch (err) {
                    console.error(err.message);
                    if (err.message === 'no_session')
                        window.location.href = '/';
                    this.qty++;
                }
            }
        },

        async removeAll() {
            var old = this.qty;
            this.qty = 0;
            try {
                const res = await fetch('/order/removeallitem', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: `id=${this.menuItemId}&__RequestVerificationToken=${encodeURIComponent(getToken())}`
                });
                const data = await res.json();

                if (data.error)
                    throw new Error(data.error);

                document.dispatchEvent(new CustomEvent('basket-updated-local', { detail: { basketCount: data.basketCount } }));
            } catch (err) {
                console.error(err.message);
                if (err.message === 'no_session')
                    window.location.href = '/';
                this.qty = old;
            }
            
        }
    }
}

function getToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]').value;
}