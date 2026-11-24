function BookingManagement() {
  const params = new URLSearchParams(window.location.hash.slice(1));
  const token = params.get("token");

  return (
    <section className="booking-management">
      <h2>Manage your reservation</h2>

      <p>
        {token ? "Management token received." : "Management token is missing."}
      </p>
    </section>
  );
}

export default BookingManagement;
