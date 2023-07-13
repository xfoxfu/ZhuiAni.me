import { Alert, AlertIcon, AlertDescription, Tag, HStack, Icon, Link, Progress, Text } from "@chakra-ui/react";
import React from "react";
import api from "../../api";
import { LinkIcon, ViewIcon } from "@chakra-ui/icons";

export const ItemLinks: React.FunctionComponent<{ id: number }> = ({ id }) => {
  const { data: links, error } = api.itemLink.useGetItemLinks(id);

  return (
    <>
      {error && (
        <Alert status="error">
          <AlertIcon />
          <AlertDescription>{error.message}</AlertDescription>
        </Alert>
      )}
      {!links && <Progress width="100%" size="xs" colorScheme="teal" isIndeterminate />}
      <HStack spacing={1}>
        {links &&
          links.map((i) => (
            <Link key={i.id} href={i.address} isExternal>
              <Tag variant="solid" colorScheme="teal">
                {i.mime_type === "text/html;kind=bgm.tv" ? (
                  <HStack spacing={1}>
                    <LinkIcon />
                    <Text>bgm.tv</Text>
                  </HStack>
                ) : i.mime_type === "text/html;kind=video" ? (
                  <ViewIcon />
                ) : (
                  <Icon />
                )}
              </Tag>
            </Link>
          ))}
      </HStack>
    </>
  );
};
