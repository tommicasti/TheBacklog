import React, { useState } from 'react';
import { REGISTER_API_URL } from '../util/constants';

export default function Register() {
    const [success, setSuccess] = useState(false);
    const [error, setError] = useState(false);
    
    const registerUser = async (username: any, password: any) => {        
        try{
            let response = await fetch(REGISTER_API_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    Username: username,
                    Password: password,
                }),
            });

            if(response.ok){
                setSuccess(true);
                setError(false);
            } else{
                setError(true);
                setSuccess(false);
            }
        }catch(err){
            setError(true);
            setSuccess(false);
            console.error(err);
        }
    }

    const validateForm = (e: React.FormEvent) => {
        e.preventDefault();
        const username = (e.target as HTMLFormElement).usernameSignUp.value;
        const password = (e.target as HTMLFormElement).passwordSignUp.value;
        const confirmPassword = (e.target as HTMLFormElement).confirmPassword.value;

        if(password === confirmPassword){
            registerUser(username, password)
        }else{
            alert("Passwords don't match");
        }
    }

    return (
        <div>
            <h2>Create an account</h2>
            <p>Sign up and start manage your game backlog!</p>
            {success && <p className="success-message">Registration successful! You can now sign in.</p>}
            {error && <p className="error-message">Registration failed. Please try again.</p>}
            <form id="signup-form" className="form" onSubmit={e => validateForm(e)}>
                <input type="text" name="usernameSignUp" placeholder="Username" required />
                <input type="password" name="passwordSignUp" placeholder="Password" required />
                <input type="password" name="confirmPassword" placeholder="Confirm Password" required />
                <button type="submit" className="standard-button dark-border"><span>Sign Up</span></button>
            </form>
        </div>
    );
}