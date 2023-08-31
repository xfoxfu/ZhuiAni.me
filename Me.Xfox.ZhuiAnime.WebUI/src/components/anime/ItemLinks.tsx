import { Alert, AlertIcon, AlertDescription, Tag, HStack, Icon, Link, Progress, Text, Tooltip } from "@chakra-ui/react";
import React from "react";
import api from "../../api";
import MIMEType from "whatwg-mimetype";
import { IoOpenOutline, IoLinkOutline, IoVideocamOutline } from "react-icons/io5";

export const ItemLinks: React.FunctionComponent<{ id: number }> = ({ id }) => {
  const { data: links, error } = api.api.useItemLinkList(id);

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
              <Tooltip label={i.address}>
                <Tag variant="solid" colorScheme="teal">
                  {i.mime_type === "text/html;kind=bgm.tv" ? (
                    <HStack spacing={1}>
                      <Icon as={IoOpenOutline} />
                      <Text>bgm.tv</Text>
                    </HStack>
                  ) : MIMEType.parse(i.mime_type)?.type === "video" ||
                    MIMEType.parse(i.mime_type)?.parameters.get("kind") === "text/html;kind=video" ? (
                    <Icon as={IoVideocamOutline} />
                  ) : (
                    <Icon as={IoLinkOutline} />
                  )}
                </Tag>
              </Tooltip>
            </Link>
          ))}
      </HStack>
    </>
  );
};
