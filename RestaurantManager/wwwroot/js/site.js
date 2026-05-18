function basketState() {
    return {
        basketCount: parseInt(document.querySelector('meta[name="basket-count"]')?.content ?? '0'),

        init() {
            document.addEventListener('basket-updated-local', (e) => {
                this.basketCount = e.detail.basketCount;
            });

            document.addEventListener('basket-updated', (e) => {
                this.basketCount = e.detail.basketCount;
            });
        }
    }
}

const tableNumber = document.querySelector('meta[name="table-number"]')?.content;

if (tableNumber) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/table")
        .withAutomaticReconnect()
        .build();

    connection.on("BasketUpdated", (data) => {
        document.dispatchEvent(new CustomEvent('basket-updated', { detail: { quantities: data.quantities, basketCount: data.basketCount } }));
    });

    connection.on("OrderPlaced", () => {
        if (window.location.pathname.toLowerCase().includes('/basket')) {
            window.location.reload();
        }
    });

    connection.on("SessionClosed", () => {
        window.location.href = "/session/ended";
    });

    connection.start()
        .then(() => connection.invoke("JoinTable", tableNumber))
        .catch(err => console.error("SignalR connection failed:", err));

    window.tableConnection = connection;
}