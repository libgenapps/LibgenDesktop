<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <xsl:for-each select="(//a[contains(@href, '/item/adv/')])[1]">
        <xsl:text>https://sci.libgen.me</xsl:text>
        <xsl:value-of select="@href" />
    </xsl:for-each>
  </xsl:template>
</xsl:transform>
