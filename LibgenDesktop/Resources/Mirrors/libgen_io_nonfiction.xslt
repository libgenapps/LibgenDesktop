<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <xsl:choose>
      <xsl:when test="//a[starts-with(@href, 'http://libgen.io/get.php')]">
        <xsl:for-each select="(//a[starts-with(@href, 'http://libgen.io/get.php')])[1]">
          <xsl:value-of select="@href" />
        </xsl:for-each>
      </xsl:when>
      <xsl:when test="//a[starts-with(@href, 'http://download.libgen.io/get/')]">
        <xsl:for-each select="(//a[starts-with(@href, 'http://download.libgen.io/get/')])[1]">
          <xsl:value-of select="@href" />
        </xsl:for-each>
      </xsl:when>
    </xsl:choose>
  </xsl:template>
</xsl:transform>
