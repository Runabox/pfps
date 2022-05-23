import { Container, Center, Text, FormControl, FormLabel, Input, Button, Spinner, Box } from '@chakra-ui/react';
import Header from '../comps/header';
import Footer from '../comps/footer';

import { useEffect, useState } from 'react';
import { FaDiscord } from 'react-icons/fa';
import { Store } from 'react-notifications-component';
import Captcha from '../comps/captcha';

interface LoginError {
    type: number,
    error?: string,
}

const Login = () => {
    let [email, setEmail] = useState("");
    let [password, setPassword] = useState("");
    let [reCAPTCHA, setReCAPTCHA] = useState("");
    let [loading, setLoading] = useState(true);

    useEffect(() => {
        if (localStorage.getItem("token")) {
            window.location.href = '/?ref=logged_in';
            return;
        }

        setLoading(false);
    }, []);

    if (loading) {
        return (
            <Container maxW="container.xl">
                <Header loggedIn={false} />

                <Center>
                    <Spinner marginTop={100} />
                </Center>

                <Footer fixed={true} />
            </Container>
        );
    }

    return (
        <Container maxW="container.xl">
            <Header loggedIn={false} />

            <Center marginTop={30}>
                <Box
                    borderWidth="1px"
                    borderRadius="15px"
                    width={500}
                    height={650}
                    textAlign="center"
                    padding={20}
                    shadow="md"
                >
                    <Text fontSize="300%">Login</Text>

                    <form>
                        <FormControl isRequired>
                            <FormLabel>Email</FormLabel>
                            <Input
                                type="email"
                                placeholder="email@example.com"
                                size="lg"
                                onChange={event => setEmail(event.currentTarget.value)}
                            />
                        </FormControl>
                        <FormControl isRequired mt={6}>
                            <FormLabel>Password</FormLabel>
                            <Input
                                type="password"
                                placeholder="*******"
                                size="lg"
                                onChange={event => setPassword(event.currentTarget.value)}
                            />
                        </FormControl>
                        <Center
                            marginTop={3}
                        >
                            <Captcha
                                response={(res: string) => {
                                    setReCAPTCHA(res);
                                }}
                            />
                        </Center>

                        <Button
                            variant="outline"
                            width="full"
                            mt={4}
                            onClick={async () => {
                                var res = await handleSubmit({ email: email, password: password, reCaptchaResponse: reCAPTCHA });

                                if (res.type === 1) {
                                    // error logging in
                                    Store.addNotification({
                                        title: "Error logging in.",
                                        message: res.error!,
                                        type: "danger",
                                        insert: "top",
                                        container: "bottom-right",
                                        animationIn: ["animate__animated", "animate__fadeIn"],
                                        animationOut: ["animate__animated", "animate__fadeOut"],
                                        dismiss: {
                                            duration: 5000,
                                            onScreen: true
                                        }
                                    });
                                } else if (res.type === 2) {
                                    // unknown error
                                    Store.addNotification({
                                        title: "Error logging in.",
                                        message: "An unknown error has occured. Please try again.",
                                        type: "danger",
                                        insert: "top",
                                        container: "bottom-right",
                                        animationIn: ["animate__animated", "animate__fadeIn"],
                                        animationOut: ["animate__animated", "animate__fadeOut"],
                                        dismiss: {
                                            duration: 5000,
                                            onScreen: true
                                        }
                                    });
                                }
                            }}
                            bg="purple.500"
                            color="white"
                            _hover={{ bg: 'purple.700' }}
                        >
                            Sign In
                        </Button>
                        <FormLabel fontSize="75%">No account? <a onClick={() => alert('This action is currently not supported. Please login using discord.')} href="#"><u>Register</u></a></FormLabel>
                        <Text>or</Text>
                        <Button
                            as="a"
                            variant="outline"
                            width="full"
                            mt={4}
                            bg="#6577E6"
                            color="white"
                            href="https://discord.com/api/oauth2/authorize?client_id=974470244410200174&redirect_uri=https%3A%2F%2Falpha.pfps.lol%2Fdiscord%2Flogin&response_type=code&scope=identify%20email"
                            _hover={{ bg: '#3f4a91' }}
                        >
                            <FaDiscord style={{ marginRight: 3, width: '1.5vw' }} /> Login with Discord
                        </Button>
                    </form>
                </Box>

            </Center>

            <Footer fixed={true} />
        </Container>
    );
};

const handleSubmit = async ({ email, password, reCaptchaResponse }: { email: string; password: string, reCaptchaResponse: string }): Promise<LoginError> => {
    if (!email || !password) {
        var ret: LoginError = {
            type: 1,
            error: "No email or password provided.",
        };

        return ret;
    }

    if (!reCaptchaResponse) {
        var ret: LoginError = {
            type: 1,
            error: "Please complete the reCAPTCHA!",
        };

        return ret;
    }

    let res = await fetch('https://api.pfps.lol/api/v1/login', {
        body: JSON.stringify({
            email,
            password
        }),
        headers: {
            'Content-Type': 'application/json',
            'recaptcha-response': reCaptchaResponse
        },
        method: 'post',
    });

    let result = await res.json();
    if (result.error) {
        var ret: LoginError = {
            type: 1,
            error: result.error,
        };

        return ret;
    }

    if (!result.token) {
        var ret: LoginError = {
            type: 2,
        };

        return ret;
    }

    localStorage.setItem("token", result.token);
    window.location.href = '/';

    var ret: LoginError = {
        type: 0,
    };

    return ret;
}

export default Login;