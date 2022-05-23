import { Container, Image, Text, Center, Button, Grid, GridItem, Badge, IconButton, Tooltip, Box, SimpleGrid } from '@chakra-ui/react';
import { StarIcon } from '@chakra-ui/icons';
import { Upload } from '../pages/index';
import { useState } from 'react';

const UploadSingleCard = ({ upload, adminView, token }: { upload: Upload, adminView?: boolean, token?: string }) => {
    let [removed, setRemoved] = useState(false);

    if (upload === undefined || removed) {
        return (<></>);
    }

    if (adminView) {
        return (
            <Box
                shadow='md'
                borderRadius='xl'
                borderWidth='1px'
                height={450}
                marginTop={5}
                p={10}
                color="black"
                textAlign="center"
                position="relative"
            >
                <Container
                    marginTop={-5}
                >
                    <Text fontSize="150%" marginTop={-1}>{upload.title.length <= 26 ? upload.title : `${upload.title.slice(0, 26)}...`}</Text>

                    {Tags({ upload: upload, matching: false })}
                    <br />
                    <br />

                    <Center>
                        <Container
                            position="absolute"
                            bottom={5}
                        >
                            <Tooltip hasArrow label="Download" placement="top">
                                <IconButton
                                    as="a"
                                    target="_blank"
                                    href={upload.urls[0]}
                                    aria-label='Download profile picture'
                                    icon={
                                        <Image
                                            src={upload.urls[0]}
                                            borderStyle="solid"
                                            borderWidth="2px"
                                            borderColor="purple.500"
                                            borderRadius="10px"
                                            width={150}
                                            height={150}
                                        />
                                    }
                                    width={150}
                                    objectFit="cover"
                                    height={150}
                                    _hover={{ opacity: '75%' }}
                                />
                            </Tooltip>

                            <Center marginTop={3}>
                                <Button as="a" bg="green.400" _hover={{ bg: 'green.700' }} color="white" width={100} onClick={() => {
                                    approveUpload({ id: upload.id, token })
                                        .then(() => {
                                            setRemoved(true);
                                        });
                                }}>Approve</Button>
                                <Button as="a" bg="red.400" _hover={{ bg: 'red.700' }} color="white" marginLeft={2} width={100} onClick={() => {
                                    removeUpload({ id: upload.id, token })
                                        .then(() => {
                                            setRemoved(true);
                                        });
                                }}>Remove</Button>
                            </Center>
                        </Container>
                    </Center>
                </Container>
            </Box>
        );
    }

    return (
        <Tooltip hasArrow label={`View ${upload.title}`} placement="top">
            <IconButton
                as="a"
                aria-label='View profile picture'
                icon={
                    <Image
                        src={upload.urls[0]}
                        borderStyle="solid"
                        borderWidth="2px"
                        borderColor="purple.500"
                        borderRadius="10px"
                        width={300}
                        objectFit="cover"
                        height={300}
                    />
                }
                width={300}
                height={300}
                _hover={{ opacity: '75%' }}
                href={`/view/${upload.id}`}
            />
        </Tooltip>
    );
}

const UploadMatchingCard = ({ upload, adminView, token }: { upload: Upload, adminView?: boolean, token?: string }) => {
    let [removed, setRemoved] = useState(false);

    if (upload === undefined || removed) {
        return (<></>);
    }

    if (adminView) {
        return (
            <Box
                p={10}
                shadow='md'
                borderRadius='xl'
                borderWidth='1px'
                height={550}
                marginTop={5}
                position="relative"
            >
                <Grid
                    templateColumns='repeat(3, 1fr)'
                >
                    <GridItem
                        colSpan={3}
                    >
                        <Text fontSize="175%" marginTop={-1}>{upload.title.length <= 30 ? upload.title : `${upload.title.slice(0, 30)}...`}</Text>
                        {upload.description === null ? <Text><i>No description is available for this post.</i></Text> : <Text>{new String(upload.description).length <= 52 ? upload.description : `${upload.description.slice(0, 52)}...`}</Text>}

                        {Tags({ upload: upload, matching: true })}
                    </GridItem>

                    <GridItem
                        colStart={4}
                        colEnd={5}
                        textAlign="right"
                    >
                        <Button width={100} bg="green.400" _hover={{ bg: 'green.700' }} color="white" onClick={() => {
                            approveUpload({ id: upload.id, token })
                                .then(() => {
                                    setRemoved(true);
                                });
                        }}>Approve</Button>

                        <Button width={100} marginLeft={2} bg="red.400" _hover={{ bg: 'red.700' }} color="white" onClick={() => {
                            removeUpload({ id: upload.id, token })
                                .then(() => {
                                    setRemoved(true);
                                });
                        }}>Remove</Button>
                    </GridItem>
                </Grid>
                <br />
                <Center>
                    <Grid
                        templateColumns='repeat(2, 1fr)'
                        gap={4}
                        position="absolute"
                        bottom={30}
                    >
                        <GridItem
                            colSpan={1}
                        >
                            <Tooltip hasArrow label="Download" placement="top">
                                <IconButton
                                    as="a"
                                    target="_blank"
                                    href={upload.urls[0]}
                                    aria-label='Download profile picture'
                                    icon={
                                        <Image
                                            src={upload.urls[0]}
                                            borderStyle="solid"
                                            borderWidth="3px"
                                            borderColor="purple.500"
                                            borderRadius="10px"
                                            width={300}
                                            objectFit="cover"
                                            height={250}
                                        />
                                    }
                                    width={250}
                                    height={250}
                                    _hover={{ opacity: '75%' }}
                                />
                            </Tooltip>
                        </GridItem>

                        <GridItem
                            colSpan={1}
                        >
                            <Tooltip hasArrow label="Download" placement="top">
                                <IconButton
                                    as="a"
                                    target="_blank"
                                    href={upload.urls[1]}
                                    aria-label='Download profile picture'
                                    icon={
                                        <Image
                                            src={upload.urls[1]}
                                            borderStyle="solid"
                                            borderWidth="3px"
                                            borderColor="purple.500"
                                            borderRadius="10px"
                                            width={300}
                                            objectFit="cover"
                                            height={250}
                                        />
                                    }
                                    width={250}
                                    height={250}
                                    _hover={{ opacity: '75%' }}
                                />
                            </Tooltip>
                        </GridItem>
                    </Grid>
                </Center>
            </Box>
        );
    }

    return (
        <Box
            p={10}
            shadow='lg'
            borderRadius='xl'
            borderWidth='1px'
            minH={[900, 450]}
            w='100%'
            position="relative"
        >
            <Grid
                templateColumns='repeat(3, 1fr)'
            >
                <GridItem
                    colSpan={3}
                >
                    <Text fontSize="175%" marginTop={-1}>{upload.title.length <= 30 ? upload.title : `${upload.title.slice(0, 30)}...`}</Text>

                    {Tags({ upload: upload, matching: true })}
                </GridItem>

                <GridItem
                    colStart={4}
                    colEnd={5}
                    textAlign="right"
                >
                    <Button as="a" width={125} href={`/view/${upload.id}`}>View</Button>
                    <Tooltip hasArrow label="Favorite" placement="top">
                        <IconButton
                            marginLeft={2}
                            color="white"
                            bg="yellow.400"
                            aria-label='Favorite profile picture'
                            icon={<StarIcon />}
                        />
                    </Tooltip>
                    <Text><i><b>{upload.views}</b> views</i></Text>
                </GridItem>
            </Grid>
            <br />
            <Center>
                <SimpleGrid
                    spacingY={[0, 5]}
                    position="absolute"
                    bottom={30}
                    columns={2}
                    minChildWidth={['300px', '100px']}
                    borderStyle="solid"
                    borderWidth="4px"
                    borderColor="purple.500"
                    borderRadius="10px"
                    width={[260, '82.7%']}
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
                                        src={upload.urls[0]}
                                        width={300}
                                        height={250}
                                        objectFit="cover"
                                    />
                                }
                                width={250}
                                marginRight={[12, 0]}
                                height={250}
                                _hover={{ opacity: '75%' }}
                            />
                        </Tooltip>
                    </Center>

                    <Center>
                        <Tooltip hasArrow label="Download" placement="top">
                            <IconButton
                                as="a"
                                target="_blank"
                                href={upload.urls[1]}
                                aria-label='Download profile picture'
                                icon={
                                    <Image
                                        src={upload.urls[1]}
                                        width={300}
                                        objectFit="cover"
                                        height={250}
                                    />
                                }
                                width={250}
                                marginRight={[12, 0]}
                                height={250}
                                _hover={{ opacity: '75%' }}
                            />
                        </Tooltip>
                    </Center>
                </SimpleGrid>
            </Center>

        </Box >
    );
};

const approveUpload = async ({ id, token }: { id: string, token?: string }) => {
    if (token === undefined) {
        return false;
    }

    let res = await fetch(`https://api.pfps.lol/api/v1/uploads/${id}/approve`, {
        method: 'post',
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });

    if (res.status !== 200) {
        return false;
    }

    return true;
}

const removeUpload = async ({ id, token, reason }: { id: string, token?: string, reason?: string }) => {
    if (token === undefined) {
        return false;
    }

    let res = await fetch(`https://api.pfps.lol/api/v1/uploads/${id}/disapprove`, {
        method: 'post',
        headers: {
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ reason })
    });

    if (res.status !== 204) {
        return false;
    }

    return true;
}

const Tags = ({ upload, matching }: { upload: Upload; matching: boolean }) => {
    let tags = [];
    let i = 1;
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
                {tag.length <= 10 ? tag : `${tag.slice(0, 10)}...`}
            </Badge>
        );

        i++;
    });

    return tags;
};

export {
    UploadMatchingCard,
    UploadSingleCard,
};

export default UploadMatchingCard;