import React from 'react';
import { REGISTER_API_URL } from '../util/constants';

export default function Register() {
    
    const registerUser = async (email: any, password: any) => {        
        try{
            let response = await fetch(REGISTER_API_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    Username: email,
                    Password: password,
                }),
            });

            if(response.ok){
                let data = await response.json;
                console.log(data);
            }
        }catch(err){
            console.error(err);
        }
    }

    const validateForm = (e: React.FormEvent) => {
        e.preventDefault();
        const email = (e.target as HTMLFormElement).emailSignUp.value;
        const confirmEmail = (e.target as HTMLFormElement).confirmEmail.value;
        const password = (e.target as HTMLFormElement).passwordSignUp.value;
        const confirmPassword = (e.target as HTMLFormElement).confirmPassword.value;

        if(email === confirmEmail){
            if(password === confirmPassword){
                registerUser(email, password)
            }else{
                alert("Passwords don't match");
            }
        }else{
            alert("Emails don't match");
        }
    }

    return (
        <div>
            <h2>Create an account</h2>
            <p>Sign up and start manage your game backlog!</p>
            <form id="signup-form" className="form" onSubmit={e => validateForm(e)}>
                <input type="email" name="emailSignUp" placeholder="Email" required />
                <input type="email" name="confirmEmail" placeholder="Confirm Email" required />
                <input type="password" name="passwordSignUp" placeholder="Password" required />
                <input type="password" name="confirmPassword" placeholder="Confirm Password" required />
                <button type="submit" className="standard-button dark-border"><span>Sign Up</span></button>
            </form>
        </div>
    );
}