import {
  useDisclosure,
  Button,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalCloseButton,
  ModalBody,
  ModalFooter,
  Icon,
  IconButton,
  FormControl,
  FormLabel,
  Input,
  VStack,
  Switch,
  FormErrorMessage,
  useToast,
  InputRightAddon,
  InputGroup,
  InputRightElement,
} from "@chakra-ui/react";
import React, { useEffect } from "react";
import { IoCodeWorkingOutline, IoCreateOutline } from "react-icons/io5";
import { useForm, SubmitHandler } from "react-hook-form";
import api, { ApiError } from "../../api";
import escape from "escape-string-regexp";

type Inputs = {
  bangumi: number;
  target: string;
  regex: string;
  match_group_ep: number;
  enabled: boolean;
};

export const Edit: React.FunctionComponent<{ id?: number }> = ({ id }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const { data: task } = api.pikPak.useGetModulesPikpakJob(id ?? 0, {}, !!id);
  const toast = useToast();
  const {
    register,
    handleSubmit,
    formState: { errors: formErrs, isDirty },
    setValue,
    getValues,
  } = useForm<Inputs>({
    defaultValues: { enabled: true },
  });
  const onSubmit: SubmitHandler<Inputs> = async (inputs) => {
    try {
      if (id) {
        await api.pikPak.postModulesPikpakJob(id, {
          bangumi: inputs.bangumi,
          target: inputs.target,
          regex: inputs.regex,
          match_group_ep: inputs.match_group_ep,
          enabled: inputs.enabled,
        });
        await api.pikPak.mutateGetModulesPikpakJob(id);
      } else {
        await api.pikPak.postModulesPikpakJobs({
          bangumi: inputs.bangumi,
          target: inputs.target,
          regex: inputs.regex,
          match_group_ep: inputs.match_group_ep,
        });
      }
      await api.pikPak.mutateGetModulesPikpakJobs();
      toast({
        title: "Job created.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      onClose();
    } catch (err) {
      toast({
        title: (err as Error | ApiError).message,
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    }
  };
  useEffect(() => {
    if (!isDirty && task) {
      setValue("bangumi", task.bangumi);
      setValue("target", task.target);
      setValue("regex", task.regex);
      setValue("match_group_ep", task.match_group_ep);
      setValue("enabled", task.enabled);
    }
  }, [isDirty, setValue, task]);
  const onEscapeRegex = () => {
    setValue("regex", escape(getValues("regex")));
  };

  return (
    <>
      {id ? (
        <IconButton aria-label="Edit" icon={<Icon as={IoCreateOutline} />} onClick={onOpen} />
      ) : (
        <Button colorScheme="teal" onClick={onOpen} width="fit-content">
          Create
        </Button>
      )}

      <Modal isOpen={isOpen} onClose={onClose} size="xl">
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Edit PikPak Task</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <VStack spacing="2">
              <FormControl isInvalid={!!formErrs.bangumi}>
                <FormLabel>Bangumi ID</FormLabel>
                <Input type="number" {...register("bangumi", { required: true, valueAsNumber: true })} />
                <FormErrorMessage>{formErrs.bangumi?.message}</FormErrorMessage>
              </FormControl>
              <FormControl isInvalid={!!formErrs.target}>
                <FormLabel>Target Folder</FormLabel>
                <Input {...register("target", { required: true })} />
                <FormErrorMessage>{formErrs.target?.message}</FormErrorMessage>
              </FormControl>
              <FormControl isInvalid={!!formErrs.regex}>
                <FormLabel>RegExp</FormLabel>
                <InputGroup>
                  <Input {...register("regex", { required: true })} />
                  <InputRightElement>
                    <IconButton
                      icon={<IoCodeWorkingOutline />}
                      aria-label="Escape regex"
                      onClick={onEscapeRegex}
                      size="sm"
                    />
                  </InputRightElement>
                </InputGroup>
                <FormErrorMessage>{formErrs.regex?.message}</FormErrorMessage>
              </FormControl>
              <FormControl isInvalid={!!formErrs.match_group_ep}>
                <FormLabel>Episode ID Group ID</FormLabel>
                <Input type="number" {...register("match_group_ep", { required: true, valueAsNumber: true })} />
                <FormErrorMessage>{formErrs.match_group_ep?.message}</FormErrorMessage>
              </FormControl>
              <FormControl display="flex" alignItems="center">
                <FormLabel mb={0}>Enabled</FormLabel>
                <Switch {...register("enabled", { disabled: !id })} />
              </FormControl>
            </VStack>
          </ModalBody>

          <ModalFooter>
            <Button onClick={onClose} mr="2">
              Close
            </Button>
            {/* eslint-disable-next-line @typescript-eslint/no-misused-promises */}
            <Button colorScheme="teal" onClick={handleSubmit(onSubmit)} isDisabled={!isDirty}>
              Save
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
};
