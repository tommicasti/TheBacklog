export default function Register() {
    return (
        <div>
            <h2>Create an account</h2>
            <p>Sign up and start manage your game backlog!</p>
            <form id="signup-form" className="form">
                <input type="email" name="email" placeholder="Email" required />
                <input type="email" name="confirm-email" placeholder="Confirm Email" required />
                <input type="password" name="password" placeholder="Password" required />
                <input type="password" name="confirm-password" placeholder="Confirm Password" required />
                <button type="submit">Sign Up</button>
            </form>
        </div>
    );
}