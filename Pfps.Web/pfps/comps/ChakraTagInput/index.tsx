import { forwardRef, useCallback, useState } from 'react'
import type { ForwardedRef, KeyboardEvent, SyntheticEvent } from 'react'
import { Button, Flex, Input, Wrap, WrapItem, WrapItemProps, WrapProps } from '@chakra-ui/react'
import type { InputProps, TagProps, TagLabelProps, TagCloseButtonProps } from '@chakra-ui/react'
import { Store } from 'react-notifications-component';

import { maybeCall } from './maybe'
import type { MaybeFunc } from './maybe'
import ChakraTagInputTag from './Tag'

type MaybeIsInputProps<P> = MaybeFunc<[isInput: boolean, index?: number], P>
type MaybeTagProps<P> = MaybeFunc<[tag: string, index?: number], P>

export type ChakraTagInputProps = InputProps & {
    tags?: string[]
    onTagsChange?(event: SyntheticEvent, tags: string[]): void
    onTagAdd?(event: SyntheticEvent, value: string): void
    onTagRemove?(event: SyntheticEvent, index: number): void

    vertical?: boolean
    addKeys?: string[]

    wrapProps?: WrapProps,
    wrapItemProps?: MaybeIsInputProps<WrapItemProps>,
    tagProps?: MaybeTagProps<TagProps>
    tagLabelProps?: MaybeTagProps<TagLabelProps>
    tagCloseButtonProps?: MaybeTagProps<TagCloseButtonProps>
}

export default forwardRef(function ChakraTagInput({
    tags = [],
    onTagsChange,
    onTagAdd,
    onTagRemove,
    vertical = false,
    addKeys = ['Enter'],
    wrapProps,
    wrapItemProps,
    tagProps,
    tagLabelProps,
    tagCloseButtonProps,
    ...props
}: ChakraTagInputProps, ref: ForwardedRef<HTMLInputElement>) {
    let [tagValue, setTagValue] = useState("");
    const addTag = useCallback(
        (event: SyntheticEvent, tag: string) => {
            if(tags.concat([tag]).length > 8) {
                Store.addNotification({
                    title: "Error adding tag",
                    message: "Max. 8 tags",
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
                return;
            }

            if(tags.includes(tag)) {
                Store.addNotification({
                    title: "Error adding tag",
                    message: "Cannot have duplicate tags.",
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
                return;
            }

            onTagAdd?.(event, tag)
            if (event.isDefaultPrevented()) return

            onTagsChange?.(event, tags.concat([tag]))
        },
        [tags, onTagsChange, onTagAdd]
    )
    const removeTag = useCallback(
        (event: SyntheticEvent, index: number) => {
            onTagRemove?.(event, index)
            if (event.isDefaultPrevented()) return

            onTagsChange?.(event, [...tags.slice(0, index), ...tags.slice(index + 1)])
        },
        [tags, onTagsChange, onTagRemove]
    )
    const handleRemoveTag = useCallback(
        (index: number) => (event: SyntheticEvent) => {
            removeTag(event, index)
        },
        [removeTag]
    )
    const onKeyDown = props.onKeyDown
    const handleKeyDown = useCallback(
        (event: KeyboardEvent<HTMLInputElement>) => {
            onKeyDown?.(event)

            if (event.isDefaultPrevented()) return
            if (event.isPropagationStopped()) return

            const { currentTarget, key } = event
            const { selectionStart, selectionEnd } = currentTarget
            let lC = currentTarget.value.toLocaleLowerCase();
            if (addKeys.indexOf(key) > -1 && (currentTarget.value)) {
                if (lC === "matching" || lC === "single") {
                    Store.addNotification({
                        title: "Error adding tag",
                        message: "You cannot use \"matching\" or \"single\" in a tag.",
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
                    if (!event.isDefaultPrevented()) {
                        setTagValue('');
                    }
                    event.preventDefault()
                } else if (lC.includes("#") || lC.includes(",")) {
                    Store.addNotification({
                        title: "Error adding tag",
                        message: "Tag cannot include invalid character (# or ,)",
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
                    if (!event.isDefaultPrevented()) {
                        setTagValue('');
                    }
                    event.preventDefault()
                } else {
                    addTag(event, currentTarget.value)
                    if (!event.isDefaultPrevented()) {
                        setTagValue('');
                    }
                    event.preventDefault()
                }
            } else if (key === 'Backspace' && tags.length > 0 && selectionStart === 0 && selectionEnd === 0) {
                removeTag(event, tags.length - 1)
            }
        },
        [addKeys, tags.length, addTag, removeTag, onKeyDown]
    )
    const handleSubmit = (e: SyntheticEvent, value: string) => {
        let lC = value.toLocaleLowerCase();
        if(lC === "") {
            return;
        }
        
        if (lC === "matching" || lC === "single") {
            Store.addNotification({
                title: "Error adding tag",
                message: "You cannot use \"matching\" or \"single\" in a tag.",
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
        } else if (lC.includes("#") || lC.includes(",")) {
            Store.addNotification({
                title: "Error adding tag",
                message: "Tag cannot include invalid character (# or ,)",
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
        } else {
            addTag(e, value)
        }
    }

    return (
        <>
            <Wrap marginBottom={2}>
                {tags.map((tag, index) => (
                    <WrapItem {...maybeCall(wrapItemProps, false, index)} key={index}>
                        <ChakraTagInputTag
                            onRemove={handleRemoveTag(index)}
                            tagLabelProps={maybeCall(tagLabelProps, tag, index)}
                            tagCloseButtonProps={maybeCall(tagCloseButtonProps, tag, index)}
                            colorScheme={props.colorScheme}
                            size={props.size}
                            {...maybeCall(tagProps, tag, index)}
                        >
                            {tag}
                        </ChakraTagInputTag>
                    </WrapItem>
                ))}
            </Wrap>
            <Flex>
                <Input placeholder="Enter Tag" maxLength={25} {...props} value={tagValue} onKeyDown={handleKeyDown} onChange={(e) => setTagValue(e.currentTarget.value)} ref={ref} />
                <Button width={100} marginLeft={2} onClick={(e) => {
                    handleSubmit(e, tagValue);
                    setTagValue('');
                }}>Add</Button>
            </Flex>
        </>
    )
});
