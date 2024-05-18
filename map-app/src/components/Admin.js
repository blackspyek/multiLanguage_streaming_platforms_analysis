import { Link } from "react-router-dom";
import Users from "./Users";

function Admin() {
  return (
    <section>
      <h1>Admin Dashboard</h1>
      <br />
      <Users />
      <br />
      <Link to="/">Go to Home</Link>
    </section>
  );
}

export default Admin;
