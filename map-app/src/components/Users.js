import { useState, useEffect } from "react";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
function Users() {
  const [users, setUsers] = useState();
  const axiosPrivate = useAxiosPrivate();

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();
    const getUsers = async () => {
      try {
        const response = await axiosPrivate.get("/users/all", {
          signal: controller.signal,
        });
        console.log(response.data);
        isMounted && setUsers(response.data?.users);
      } catch (err) {
        console.log(err);
      }
    };

    getUsers();

    return () => {
      // cleanup function
      isMounted = false;
      controller.abort();
    };
  }, [axiosPrivate]);

  return (
    <article>
      <h2>Users List</h2>
      {users?.length ? (
        <ul>
          {users.map((user, id) => (
            <li key={id}>{user.username}</li>
          ))}
        </ul>
      ) : (
        <p>No users found</p>
      )}
      <br />
    </article>
  );
}

export default Users;
