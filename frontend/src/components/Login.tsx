export default function Login() {
    return (
        <div>
            <h2>Sign in</h2>
            <p>Sign in with your backlog account</p>
            <form id="signin-form" className="form">
                <input type="email" name="email" placeholder="Email" required />
                <input type="password" name="password" placeholder="Password" required />
                <button type="submit">Sign In</button>
            </form>
        </div>

    );
}