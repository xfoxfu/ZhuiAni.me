import {
  Heading,
  Stack,
  Alert,
  AlertDescription,
  AlertIcon,
  Table,
  TableContainer,
  Tbody,
  Td,
  Th,
  Thead,
  Tr,
  Link,
  Button,
  Icon,
  HStack,
  Tag,
  Text,
} from "@chakra-ui/react";
import React from "react";
import Api from "../api";
import { Edit } from "../components/pikpak/Edit";
import { IoCheckmarkCircleOutline, IoCloseCircleOutline } from "react-icons/io5";

export const PikPakTasksList: React.FunctionComponent = () => {
  const { data: tasks, error } = Api.pikPak.useGetModulesPikpakJobs();
  return (
    <>
      <Heading as="h1" size="xl" color="green.700">
        Tasks
      </Heading>
      <Stack spacing="1">
        {error && (
          <Alert status="error">
            <AlertIcon />
            <AlertDescription>{error.message}</AlertDescription>
          </Alert>
        )}

        <Edit />
        <TableContainer>
          <Table variant="simple">
            <Thead>
              <Tr>
                <Th>ID</Th>
                <Th>Bangumi</Th>
                <Th>Target Folder</Th>
                <Th>RegEx</Th>
                <Th></Th>
              </Tr>
            </Thead>
            <Tbody>
              {tasks?.flat()?.map((t) => (
                <Tr key={t.id}>
                  <Td>
                    <HStack>
                      <Text>{t.id}</Text>
                      {t.enabled ? <Icon as={IoCheckmarkCircleOutline} /> : <Icon as={IoCloseCircleOutline} />}
                    </HStack>
                  </Td>
                  <Td>
                    <Link color="teal.500" href={`https://bgm.tv/subject/${t.bangumi}`} target="_blank">
                      {t.bangumi}
                    </Link>
                  </Td>
                  <Td>{t.target}</Td>
                  <Td>{t.regex}</Td>
                  <Td>
                    <Edit id={t.id} />
                  </Td>
                </Tr>
              ))}
            </Tbody>
          </Table>
        </TableContainer>
      </Stack>
    </>
  );
};
