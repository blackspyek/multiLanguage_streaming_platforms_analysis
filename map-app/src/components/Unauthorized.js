import { useNavigate } from "react-router-dom";
export default function Unauthorized() {
  const navigate = useNavigate();
  const goBack = () => navigate(-1);
  return (
    <section>
      <h1>Unauthorized</h1>
      <p>You are not authorized to view this page</p>
      <button onClick={goBack}>Go back</button>
    </section>
  );
}
