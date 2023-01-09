import { Center, Container, Spinner } from '@chakra-ui/react';
import { useState, useEffect } from 'react';
import Footer from '../../comps/footer';
import Header from '../../comps/header';

const AccountFavorites = () => {
    let [loading, setLoading] = useState(true);

    if (loading) {
        return (
            <Container maxW="container.xl">
                <Header loggedIn={true} />

                <Center>
                    <Spinner
                        marginTop={100}
                    />
                </Center>

                <Footer fixed={true} />
            </Container>
        );
    }

    return (
        <Container maxW="container.xl">
            <Header loggedIn={true} />


            <Footer fixed={true} />
        </Container>
    );
}

export default AccountFavorites;