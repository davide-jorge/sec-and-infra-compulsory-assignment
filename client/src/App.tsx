
import { useEffect, useState } from "react";
import "./index.css";

type User = {
    userId: string;
    username: string;
    role: string;
};

type LoginResponse = {
    userId: string;
    username: string;
    role: string;
    token: string;
};

const API = "https://server-cinder-surf-4618.fly.dev/User";

export default function App() {
    const [token, setToken] = useState(sessionStorage.getItem("token") || "");
    const [role, setRole] = useState(sessionStorage.getItem("role") || "");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [users, setUsers] = useState<User[]>([]);
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState("");
    const [formUsername, setFormUsername] = useState("");
    const [formPassword, setFormPassword] = useState("");
    const [formRole, setFormRole] = useState("user");
    const [showForm, setShowForm] = useState(false);

    async function login(e: React.FormEvent) {
        e.preventDefault();
        setError("");

        try {
            const response = await fetch(`${API}/Login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                setError("Invalid username or password");
                return;
            }

            const data: LoginResponse = await response.json();

            sessionStorage.setItem("token", data.token);
            sessionStorage.setItem("role", data.role);

            setToken(data.token);
            setRole(data.role);
            setPassword("");
        } catch {
            setError("Could not connect to the API");
        }
    }

    async function loadUsers() {
        try {
            const response = await fetch(`${API}/GetUsers`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            if (!response.ok) {
                setError("Could not load users");
                return;
            }

            setUsers(await response.json());
            setError("");
        } catch {
            setError("Could not connect to the API");
        }
    }

    useEffect(() => {
        if (token) {
            loadUsers();
        }
    }, [token]);

    function logout() {
        sessionStorage.removeItem("token");
        sessionStorage.removeItem("role");
        setToken("");
        setRole("");
        setUsers([]);
        setShowForm(false);
        setError("");
    }

    function openCreate() {
        setEditingId("");
        setFormUsername("");
        setFormPassword("");
        setFormRole("user");
        setShowForm(true);
        setError("");
    }

    function openEdit(user: User) {
        setEditingId(user.userId);
        setFormUsername(user.username);
        setFormPassword("");
        setFormRole(user.role);
        setShowForm(true);
        setError("");
    }

    async function saveUser(e: React.FormEvent) {
        e.preventDefault();
        setError("");

        const editing = editingId !== "";

        const body = {
            ...(editing ? { userId: editingId } : {}),
            username: formUsername,
            password: formPassword,
            role: formRole
        };

        try {
            const response = await fetch(
                editing ? `${API}/UpdateUser` : `${API}/CreateUser`,
                {
                    method: editing ? "PATCH" : "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`
                    },
                    body: JSON.stringify(body)
                }
            );

            if (!response.ok) {
                setError(`Could not save user (HTTP ${response.status})`);
                return;
            }

            setShowForm(false);
            await loadUsers();
        } catch {
            setError("Could not connect to the API");
        }
    }

    async function deleteUser(user: User) {
        if (!confirm(`Delete ${user.username}?`)) return;

        try {
            const response = await fetch(`${API}/${user.userId}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` }
            });

            if (!response.ok) {
                setError(`Could not delete user (HTTP ${response.status})`);
                return;
            }

            await loadUsers();
        } catch {
            setError("Could not connect to the API");
        }
    }

    const canManage = role === "admin" || role === "superuser";

    function canEdit(user: User) {
        return role === "admin" ||
            (role === "superuser" && user.role === "user");
    }

    if (!token) {
        return (
            <main className="login-container">
                <form onSubmit={login} className="login-form">
                    <h1>Login</h1>

                    <input
                        placeholder="Username"
                        value={username}
                        onChange={e => setUsername(e.target.value)}
                        required
                    />

                    <input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        required
                    />

                    <button type="submit">Login</button>
                    {error && <p className="error">{error}</p>}
                </form>
            </main>
        );
    }

    return (
        <main className="page">
            <header>
                <h1>User Management</h1>
                <div>
                    <span>Role: {role}</span>
                    <button onClick={logout}>Logout</button>
                </div>
            </header>

            {error && <p className="error">{error}</p>}

            <section className="users-section">
                <div className="section-header">
                    <h2>Users</h2>
                    {canManage && (
                        <button onClick={openCreate}>Create User</button>
                    )}
                </div>

                <table>
                    <thead>
                    <tr>
                        <th>Username</th>
                        <th>Role</th>
                        {canManage && <th>Actions</th>}
                    </tr>
                    </thead>
                    <tbody>
                    {users.map(user => (
                        <tr key={user.userId}>
                            <td>{user.username}</td>
                            <td>{user.role}</td>
                            {canManage && (
                                <td>
                                    {canEdit(user) && (
                                        <>
                                            <button onClick={() => openEdit(user)}>
                                                Edit
                                            </button>
                                            <button onClick={() => deleteUser(user)}>
                                                Delete
                                            </button>
                                        </>
                                    )}
                                </td>
                            )}
                        </tr>
                    ))}
                    </tbody>
                </table>
            </section>

            {showForm && (
                <section className="form-section">
                    <h2>{editingId ? "Edit User" : "Create User"}</h2>

                    <form onSubmit={saveUser}>
                        <input
                            placeholder="Username"
                            value={formUsername}
                            onChange={e => setFormUsername(e.target.value)}
                            required
                        />

                        <input
                            type="password"
                            placeholder="Password"
                            value={formPassword}
                            onChange={e => setFormPassword(e.target.value)}
                            required
                        />

                        {role === "admin" ? (
                            <select
                                value={formRole}
                                onChange={e => setFormRole(e.target.value)}
                            >
                                <option value="user">user</option>
                                <option value="superuser">superuser</option>
                                <option value="admin">admin</option>
                            </select>
                        ) : (
                            <input value="user" disabled />
                        )}

                        <div className="form-buttons">
                            <button type="submit">Save</button>
                            <button type="button" onClick={() => setShowForm(false)}>
                                Cancel
                            </button>
                        </div>
                    </form>
                </section>
            )}
        </main>
    );
}
