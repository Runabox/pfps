import { Container, Center, Spinner, Alert, HStack, Box, Text, FormLabel, Badge, SimpleGrid, Button, Tooltip, IconButton, GridItem, Grid, Image } from '@chakra-ui/react';
import useQuery from '../../comps/useQuery';

import Footer from '../../comps/footer';
import Header from '../../comps/header';

import { useState, useEffect } from 'react';
import { ArrowBackIcon, CloseIcon, StarIcon } from '@chakra-ui/icons';
import { Upload } from '../index';
import { User, userFlags } from '../account/index';

const ViewPost = () => {
    let [alertt, setAlert] = useState<any>([]);
    let [loggedIn, setLoggedIn] = useState(false);
    let [loading, setLoading] = useState(true);
    let [upload, setUpload] = useState<any>({});
    let [user, setUser] = useState<User>();
    let query = useQuery();

    useEffect(() => {
        if (!query) {
            return;
        }

        if (localStorage.getItem("token")) {
            setLoggedIn(true);
        }

        fetch('https://api.pfps.lol/api/v1/uploads/' + query.id)
            .then(res => res.json())
            .then(result => {
                if (result === undefined) {
                    return;
                }
                if (result.error) {
                    let alerts = alertt;
                    alerts = alerts.splice(alerts.length, alerts.length);
                    alerts.push(<Alert bg="red.100" borderRadius="10px" width="50%">Error retreiving post - {result.error}</Alert>);
                    return setAlert(alerts);
                }

                if (localStorage.getItem("token")) {
                    var token = localStorage.getItem("token");

                    fetch('https://api.pfps.lol/api/v1/users/@me', {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    })
                        .then(r => r.json())
                        .then(r => {
                            if (!r.error) {
                                setUser(r);
                            } else {
                                alert(`Error retrieving user ${r.error}`);
                            }

                            setUpload(result);
                            setLoading(false);
                        })
                        .catch(e => {
                            alert(`Error retrieving user ${e.type}: ${e.message}`);
                        });
                } else {
                    setUpload(result);
                    setLoading(false);
                }
            })
            .catch(e => {
                let alerts = alertt;
                alerts = alerts.splice(alerts.length, alerts.length);
                alerts.push(<Alert bg="red.100" borderRadius="10px" width="50%">Error retreiving post - {e.name} : {e.message}</Alert>);
                setAlert(alerts);
                return setLoading(false);
            });
    }, [query]);

    if (loading || (upload.urls === undefined)) {
        return (
            <Container maxW="container.xl">
                <Header loggedIn={loggedIn} />

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
            <Header loggedIn={loggedIn} />

            <Center>
                <Container maxW="container.xl" p={15}>
                    {alertt}

                    <Center>
                        <Box
                            p={19}
                            shadow='md'
                            borderRadius='10px'
                            borderWidth='2px'
                            h={[1300, 1300, 830]}
                            width='100%'
                        >
                            <Center>
                                <Container
                                    marginTop={15}
                                    width='100%'
                                >
                                    <Container
                                        width='100%'
                                    >
                                        <Text fontSize="200%">{upload.title}</Text>
                                        <Text>Posted at <i>{new Date(upload.timestamp).toUTCString()}</i></Text>
                                        <Text>{upload.views} views</Text>
                                        {Tags({ upload: upload, matching: (upload.type === 0 ? false : true) })}
                                        <FormLabel marginTop={19}>Description</FormLabel>
                                        <Text>{upload.description ? upload.description : <i>No description is available for this post.</i>}</Text>
                                    </Container>

                                    <Center marginTop={19}
                                        width='100%'
                                    >
                                        <SimpleGrid
                                            spacingY={5}
                                            spacingX={4}
                                            bottom={30}
                                            columns={2}
                                            minChildWidth={['240px']}
                                            width='88%'
                                        >
                                            <Center>
                                                <Tooltip hasArrow label="Download" placement="top">
                                                    <IconButton
                                                        as="a"
                                                        target="_blank"
                                                        href={upload.urls[0]}
                                                        aria-label='Download profile picture'
                                                        icon={
                                                            <Image
                                                                borderStyle="solid"
                                                                borderWidth="3px"
                                                                borderColor="purple.500"
                                                                borderRadius="10px"
                                                                width={300}
                                                                height={250}
                                                                objectFit="cover"
                                                                src={upload.urls[0]}
                                                            />
                                                        }
                                                        width={250}
                                                        height={250}
                                                        _hover={{ opacity: '75%' }}
                                                    />
                                                </Tooltip>
                                            </Center>

                                            {upload.type !== 0 ?
                                                <Center>
                                                    <Tooltip hasArrow label="Download" placement="top">
                                                        <IconButton
                                                            as="a"
                                                            target="_blank"
                                                            href={upload.urls[1]}
                                                            aria-label='Download profile picture'
                                                            icon={
                                                                <Image
                                                                    borderStyle="solid"
                                                                    borderWidth="3px"
                                                                    borderColor="purple.500"
                                                                    borderRadius="10px"
                                                                    width={300}
                                                                    height={250}
                                                                    objectFit="cover"
                                                                    src={upload.urls[1]}
                                                                />
                                                            }
                                                            width={250}
                                                            height={250}
                                                            _hover={{ opacity: '75%' }}
                                                        />
                                                    </Tooltip>
                                                </Center>
                                                : <></>}
                                        </SimpleGrid>

                                        <Box
                                            p={5}
                                            shadow='md'
                                            borderRadius='10px'
                                            borderWidth='1px'
                                            position='absolute'
                                            width='50%'
                                            bottom='12%'
                                        >
                                            <FormLabel>Actions</FormLabel>
                                            <SimpleGrid
                                                columns={4}
                                                spacing={4}
                                                minChildWidth='100px'
                                            >
                                                <Tooltip hasArrow placement="top" label="Add to favorites">
                                                    <Button
                                                        aria-label='Favorite post'
                                                        leftIcon={<StarIcon />}
                                                        color="white"
                                                        background="yellow.400"
                                                        _hover={{ bg: 'yellow.600' }}
                                                    >
                                                        Favorite
                                                    </Button>
                                                </Tooltip>

                                                <Button
                                                    as="a"
                                                    aria-label='Go back'
                                                    leftIcon={<ArrowBackIcon />}
                                                    color="white"
                                                    background="red.400"
                                                    _hover={{ bg: 'red.600' }}
                                                    href="/"
                                                >
                                                    Back
                                                </Button>

                                                {user ? (((user.flags & userFlags.administrator) || (user.flags & userFlags.contentModerator)) ? <Button onClick={async () => {
                                                    await fetch(`https://api.pfps.lol/api/v1/uploads/${upload.id}/disapprove`, {
                                                        method: 'post',
                                                        headers: {
                                                            Authorization: `Bearer ${localStorage.getItem("token")}`,
                                                        }
                                                    });

                                                    window.location.href = '/';
                                                }} colorScheme={'red'} leftIcon={<CloseIcon />}>Remove</Button> : <></>) : <></>}
                                            </SimpleGrid>
                                        </Box>
                                    </Center>
                                </Container>
                            </Center>
                        </Box>
                    </Center>
                </Container>
            </Center>

            <Footer fixed={false} top={50} />
        </Container>
    );
}

const Tags = ({ upload, matching }: { upload: Upload; matching: boolean }) => {
    let tags = [];
    var i = 1;
    tags.push(
        <Badge
            bg="purple.500"
            color="white"
            marginRight={2}
        >
            {matching ? "Matching" : "Single"}
        </Badge>
    );

    upload.tags.forEach((tag) => {
        if (tag === "") {
            return;
        }

        tags.push(
            <Badge
                bg="purple.500"
                color="white"
                marginRight={2}
            >
                {tag}
            </Badge>
        );

        i++;
    });

    return tags;
};

export default ViewPost;