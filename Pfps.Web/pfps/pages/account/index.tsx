import {
    Container,
    Center,
    Spinner,
    HStack,
    Box,
    VStack,
    StackDivider,
    Button,
    FormControl,
    FormLabel,
    Input,
    Text,
    SimpleGrid,
    useDisclosure,
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalCloseButton,
    ModalBody,
    ModalFooter,
    Flex,
} from '@chakra-ui/react';

import Header from '../../comps/header';
import Footer from '../../comps/footer';
import { useEffect, useState } from 'react';

import { Upload } from '../index';
import { UploadMatchingCard, UploadSingleCard } from '../../comps/uploadCard';

interface User {
    id: string,
    username: string,
    avatar: string,
    discordId?: string,
    discordUser: boolean,
    email: string,
    flags: number,
    timestamp: string,
}

const Account = () => {
    let [user, setUser] = useState<User>();
    let [loading, setLoading] = useState(true);
    let [page, setPage] = useState(0);

    // 0 = Settings
    useEffect(() => {
        if (!localStorage.getItem("token")) {
            window.location.href = "/?ref=not_logged_in";
        }

        fetch('https://api.pfps.lol/api/v1/users/@me', {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
        })
            .then(res => res.json())
            .then(result => {
                if (result.error || result === undefined) {
                    alert(`Error retreiving user: ${result.error}\nTry clearing your cache and logging in again.`);
                    return window.location.href = '/';
                }

                setUser(result);
                setLoading(false);
            });

    }, []);

    if (loading || user === undefined) {
        return (
            <Container maxW="container.xl">
                <Header loggedIn={true} />

                <Center>
                    <Spinner marginTop={100} />
                </Center>

                <Footer fixed={true} />
            </Container>
        );
    }

    return (
        <Container maxW="container.xl" tabIndex={-1}>
            <Header loggedIn={true} />

            <HStack spacing={3} marginTop={30} width='100%'>
                <Box
                    p={5}
                    shadow='md'
                    borderRadius='10px'
                    borderWidth='2px'
                    height={650}
                    width='20%'
                >
                    <VStack
                        spacing={4}
                        divider={<StackDivider borderColor='gray.200' />}
                        position="relative"
                    >
                        <Button
                            bg="gray.400"
                            color="white"
                            width="100%"
                            onClick={() => setPage(0)}
                        >
                            Settings
                        </Button>
                        {user.flags & userFlags.administrator ? <Button bg="gray.400" color="white" width="100%" onClick={() => setPage(10)}>Unapproved</Button> : <></>}
                        <Button
                            bg="red.400"
                            color="white"
                            width="100%"
                            position="absolute"
                            top={570}
                            onClick={() => {
                                localStorage.setItem("token", "");
                                window.location.href = '/?ref=logged_out';
                            }}
                        >
                            Logout
                        </Button>
                    </VStack>
                </Box>

                <Box
                    p={10}
                    shadow='md'
                    borderWidth='2px'
                    borderRadius='10px'
                    width='80%'
                    height={650}
                >
                    <Page i={page} user={user} token={localStorage.getItem("token")} />
                </Box>
            </HStack>

            <Footer fixed={false} top={50} />
        </Container>
    );
}

const Page = ({ i, user, token }: { i: number; user: User, token?: string | null }) => {
    if (i === 0) {
        return <SettingsPage user={user} />;
    }

    if (i === 10) {
        return <UnapprovedPage user={user} token={token} />;
    }

    return <SettingsPage user={user} />;
};

const SettingsPage = ({ user }: { user: User }) => {
    const { isOpen, onOpen } = useDisclosure();

    return (
        <>
            <Text fontSize="250%">
                Hi, {user.flags & userFlags.administrator ? <b style={{ color: 'red' }}><i>{"[ADMIN]"}</i></b> :
                    (user.flags & userFlags.contentModerator ? <b style={{ color: 'green' }}><i>{"[CONTENT MODERATOR]"}</i></b> : <></>)} {user.username}!
            </Text>
            <Text>Created at {new Date(user.timestamp).toUTCString()}</Text>
            {user.discordUser ? <Text><i>Discord ID - {user.discordId}</i></Text> : <></>}

            <br />
            <FormControl width="50%">
                <FormLabel>Email</FormLabel>
                <Input
                    value={user.email}
                    disabled
                />
            </FormControl>

            <FormControl
                marginTop={300}
            >
                <FormLabel>Account Actions</FormLabel>

                <SimpleGrid
                    columns={5}
                    spacing={4}
                >
                    <Button
                        bg="purple.500"
                        color="white"
                    >
                        Download Posts
                    </Button>

                    <Button
                        bg="red.500"
                        color="white"
                        onClick={onOpen}
                    >
                        Delete Account
                    </Button>
                </SimpleGrid>
            </FormControl>
            <Modal isOpen={isOpen} onClose={() => window.location.reload()}>
                <ModalOverlay display={isOpen ? "block" : "none"} />
                <ModalContent>
                    <ModalHeader>Delete Account</ModalHeader>
                    <ModalCloseButton />

                    <ModalBody>
                        <Text>Are you sure you want to delete your account?</Text>
                        <Text><b>THIS ACTION IS IRREVERSIBLE!</b></Text>
                        <br />
                        <Text><i>Note: Your posts will remain and will be consequently transferred to a SYSTEM account.</i></Text>
                    </ModalBody>

                    <ModalFooter>
                        <Button colorScheme='red' mr={3} onClick={() => alert('This action is currently not supported. If you wish to delete your account, email us at pfps@runa.live')}>
                            Confirm
                        </Button>
                    </ModalFooter>
                </ModalContent>
            </Modal>
        </>
    );
};

const UnapprovedPage = ({ user, token }: { user: User, token?: string | null }) => {
    let [loading, setLoading] = useState(true);
    let [uploads, setUploads] = useState([<></>]);
    let [refresh, setRefresh] = useState(false);

    if (token === null) {
        return (<></>);
    }

    if (loading || refresh) {
        fetch('https://api.pfps.lol/api/v1/admin/uploads/unapproved?page=0&limit=10', {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
            .then(res => res.text())
            .then(result => {
                let u = [<></>];

                let uploadsRaw: Upload[] = JSON.parse(result);
                if (uploadsRaw.length === 0) {
                    u.push(<Text><i>No results found.</i></Text>);
                }

                uploadsRaw.forEach(upload => {
                    if (upload.type === 1) {
                        u.push(<UploadMatchingCard upload={upload} adminView={true} token={token} />);
                    } else if (upload.type === 0) {
                        u.push(<UploadSingleCard upload={upload} adminView={true} token={token} />);
                    }
                });

                setUploads(u);
                setLoading(false);
                setRefresh(false);
            });

        return (
            <Center>
                <Spinner marginTop={100} />
            </Center>
        );
    }

    return (
        <Container width="100%" height="100%" overflowY="scroll" overflowX="hidden">
            <Flex>
                <Text fontSize="200%" marginBottom={5}>Unapproved uploads</Text>
                <Button
                    colorScheme='green'
                    width={100}
                    marginLeft={172}
                    onClick={() => setRefresh(true)}
                >
                    Refresh
                </Button>
            </Flex>
            {uploads}
        </Container>
    );
}

const userFlags = {
    none: 0,
    premium: 1,
    contentModerator: 2,
    administrator: 4
}

const deleteAccount = ({ user }: { user: any }) => {
    return (<></>);
}

export type {
    User
}

export {
    userFlags,
}

export default Account;